using Microsoft.AspNetCore.Http;
using PatiantMicroService.Domain.Contracts;
using PatiantMicroService.Domain.Entities;
using PatiantMicroService.Services.Specifications;
using PatiantMicroService.ServicesAbstraction.Interfaces;
using PatiantMicroService.Shared.CommonResult;
using PatiantMicroService.Shared.DTOs.AllergyDtos;
using PatiantMicroService.Shared.DTOs.MedicalRecordDtos;
using PatiantMicroService.Shared.DTOs.PatiantDtos;
using System.Security.Claims;

namespace PatiantMicroService.Services.Services
{
    public class PatientService : IPatientService
    {
        #region Constructor ➕ Attributes

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityClient _identityClient;
        private readonly IAppDbContext _appDbContext;

        public PatientService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IIdentityClient identityClient,
            IAppDbContext appDbContext)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _identityClient = identityClient;
            _appDbContext = appDbContext;
        }

        #endregion

        #region 🔐 Helpers

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userId =
                user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value
                ?? user?.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            return userId;
        }

        private string GetToken()
        {
            var authHeader = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (string.IsNullOrWhiteSpace(authHeader))
                throw new UnauthorizedAccessException("Authorization token not found");

            return authHeader;
        }

        #endregion

        #region 🧍 Get My Profile

        public async Task<PatientProfileDto> GetMyProfileAsync()
        {
            var identityUserId = GetCurrentUserId();

            var spec = new PatientByUserIdSpecification(identityUserId);

            var patient = await _unitOfWork
                .GetRepository<Patient, Guid>()
                .GetByIdAsync(spec);

            if (patient is null)
                throw new KeyNotFoundException("Patient not found");

            // ✅ هات بيانات اليوزر من Identity
            var identityResult =
                await _identityClient.GetUserByIdAsync(identityUserId);

            if (identityResult.IsFailure)
                throw new Exception(
                    identityResult.Errors.First().Description);

            var identityUser = identityResult.Value;

            return new PatientProfileDto
            {
                Id = patient.Id,

                FullName = patient.FullName,

                Address = patient.Address,

                DateOfBirth = patient.DateOfBirth,

                Gender = patient.Gender.ToString(),

                IdentityUserId = patient.IdentityUserId,

                // ✅ جاية من Identity
                Email = identityUser.Email,

                PhoneNumber = identityUser.PhoneNumber
            };
        }

        #endregion

        #region 🔥 Get My Details
        public async Task<PatientDetailsDto> GetMyDetailsAsync()
        {
            var identityUserId = GetCurrentUserId();

            var patientSpec = new PatientByUserIdSpecification(identityUserId);

            var patient = (await _unitOfWork
                .GetRepository<Patient, Guid>()
                .GetAllAsync(patientSpec))
                .FirstOrDefault();

            if (patient is null)
                throw new KeyNotFoundException("Patient not found");

            var records = await _unitOfWork
                .GetRepository<MedicalRecord, int>()
                .GetAllAsync(new MedicalRecordByPatientSpec(patient.Id));

            var allergies = await _unitOfWork
                .GetRepository<Allergy, int>()
                .GetAllAsync(new AllergyByPatientSpec(patient.Id));

            return new PatientDetailsDto
            {
                //Patient = MapToDto(patient),
                MedicalRecords = records.Select(MapMedicalRecordToDto).ToList(),
                Allergies = allergies.Select(MapAllergyToDto).ToList()
            };
        }

        #endregion

        #region ➕ Create

        public async Task<Guid> CreateAsync(CreatePatientDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var identityUserId = GetCurrentUserId();

            var patientRepo =
                _unitOfWork.GetRepository<Patient, Guid>();

            // ✅ لو موجود قبل كده رجع نفس الـ Id
            var existingPatient =
                await patientRepo.GetByIdAsync(
                    new PatientByUserIdSpecification(
                        identityUserId));

            if (existingPatient is not null)
                return existingPatient.Id;

            // ==========================
            // Identity Data
            // ==========================

            var identityUser =
                await _identityClient
                    .GetUserByIdAsync(identityUserId);

            if (identityUser.IsFailure)
                throw new Exception(
                    identityUser.Errors.First().Description);

            var user = identityUser.Value;

            // ==========================
            // Create Patient
            // ==========================

            var patient = new Patient
            {
                Id = Guid.NewGuid(),

                FullName =
                    user.DisplayName ??
                    user.Email,

                Address =
                    dto.Address ??
                    string.Empty,

                DateOfBirth =
                    dto.DateOfBirth,

                Gender =
                    (Gender)dto.Gender,

                IdentityUserId =
                    identityUserId
            };

            // ==========================
            // Allergies
            // ==========================

            if (dto.Allergies?.Any() == true)
            {
                foreach (var a in dto.Allergies)
                {
                    patient.Allergies.Add(
                        new Allergy
                        {
                            Name = a.Name,

                            Description =
                                a.Description ??
                                string.Empty
                        });
                }
            }

            // ==========================
            // Medical Records
            // ==========================

            if (dto.MedicalRecords?.Any() == true)
            {
                foreach (var m in dto.MedicalRecords)
                {
                    patient.MedicalRecords.Add(
                        new MedicalRecord
                        {
                            Diagnosis =
                                m.Diagnosis,

                            Notes =
                                m.Notes ??
                                string.Empty,

                            CreatedAt =
                                DateTime.UtcNow
                        });
                }
            }

            await patientRepo.AddAsync(patient);

            await _unitOfWork.SaveChangesAsync();

            return patient.Id;
        }

        #endregion

        #region ➕ Add MedicalRecords & Allergies

        public async Task AddMedicalDataAsync(
            AddPatientMedicalDataDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var identityUserId =
                GetCurrentUserId();

            var patientRepo =
                _unitOfWork.GetRepository<Patient, Guid>();

            var allergyRepo =
                _unitOfWork.GetRepository<Allergy, int>();

            var medicalRepo =
                _unitOfWork.GetRepository<MedicalRecord, int>();

            // ✅ هات patient بالـ identity user id
            var patient =
                await patientRepo.GetByIdAsync(
                    new PatientByUserIdSpecification(
                        identityUserId));

            if (patient is null)
                throw new KeyNotFoundException(
                    "Patient profile not found");

            // =========================
            // Allergies
            // =========================
            if (dto.Allergies?.Any() == true)
            {
                foreach (var allergy in dto.Allergies)
                {
                    await allergyRepo.AddAsync(
                        new Allergy
                        {
                            PatientId = patient.Id,

                            Name = allergy.Name,

                            Description =
                                allergy.Description
                                ?? string.Empty
                        });
                }
            }

            // =========================
            // Medical Records
            // =========================
            if (dto.MedicalRecords?.Any() == true)
            {
                foreach (var record in dto.MedicalRecords)
                {
                    await medicalRepo.AddAsync(
                        new MedicalRecord
                        {
                            PatientId = patient.Id,

                            Diagnosis =
                                record.Diagnosis,

                            Notes =
                                record.Notes
                                ?? string.Empty,

                            CreatedAt =
                                DateTime.UtcNow
                        });
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region ✏️ Update
        public async Task UpdateAsync(UpdatePatientDto dto)
        {
            var patientRepo =
                _unitOfWork.GetRepository<Patient, Guid>();

            var allergyRepo =
                _unitOfWork.GetRepository<Allergy, int>();

            var medicalRepo =
                _unitOfWork.GetRepository<MedicalRecord, int>();

            // ✅ Get logged-in user id from token
            var identityUserId = GetCurrentUserId();

            // ✅ Get patient by IdentityUserId
            var patient =
                await patientRepo.GetByIdAsync(
                    new PatientByUserIdSpecification(
                        identityUserId));

            if (patient is null)
                throw new KeyNotFoundException(
                    "Patient not found");

            // =========================
            // Identity Sync
            // =========================

            var oldIdentityData =
                await _identityClient
                    .GetUserByIdAsync(
                        patient.IdentityUserId);

            if (oldIdentityData.IsFailure)
                throw new Exception(
                    oldIdentityData.Errors
                        .First()
                        .Description);

            var identityResult =
                await _identityClient
                    .UpdatePatientAsync(
                        patient.IdentityUserId
                            .ToString(),
                        new UpdateIdentityUserRequest
                        {
                            DisplayName =
                                dto.FullName ??
                                patient.FullName,

                            Email =
                                dto.Email ??
                                oldIdentityData
                                    .Value
                                    .Email,

                            PhoneNumber =
                                dto.PhoneNumber ??
                                oldIdentityData
                                    .Value
                                    .PhoneNumber
                        },
                        GetToken());

            if (identityResult.IsFailure)
                throw new Exception(
                    identityResult.Errors
                        .First()
                        .Description);

            try
            {
                // =========================
                // Patient
                // =========================

                if (!string.IsNullOrWhiteSpace(
                        dto.FullName))
                {
                    patient.FullName =
                        dto.FullName;
                }

                if (!string.IsNullOrWhiteSpace(
                        dto.Address))
                {
                    patient.Address =
                        dto.Address;
                }

                patientRepo.Update(patient);

                // =========================
                // Allergies
                // =========================

                if (dto.Allergies is not null)
                {
                    var currentAllergies =
                        await allergyRepo
                            .GetAllAsync(
                                new AllergyByPatientSpec(
                                    patient.Id));

                    foreach (var allergyDto
                             in dto.Allergies)
                    {
                        // Update existing
                        if (allergyDto.Id.HasValue)
                        {
                            var existing =
                                currentAllergies
                                    .FirstOrDefault(
                                        x =>
                                            x.Id ==
                                            allergyDto
                                                .Id
                                                .Value);

                            if (existing is not null)
                            {
                                existing.Name =
                                    allergyDto.Name;

                                existing.Description =
                                    allergyDto
                                        .Description;

                                allergyRepo
                                    .Update(existing);
                            }
                        }
                        else
                        {
                            // Add new
                            await allergyRepo
                                .AddAsync(
                                    new Allergy
                                    {
                                        PatientId =
                                            patient.Id,

                                        Name =
                                            allergyDto
                                                .Name,

                                        Description =
                                            allergyDto
                                                .Description
                                    });
                        }
                    }
                }

                // =========================
                // Medical Records
                // =========================

                if (dto.MedicalRecords is not null)
                {
                    var currentRecords =
                        await medicalRepo
                            .GetAllAsync(
                                new MedicalRecordByPatientSpec(
                                    patient.Id));

                    foreach (var recordDto
                             in dto.MedicalRecords)
                    {
                        // Update existing
                        if (recordDto.Id.HasValue)
                        {
                            var existing =
                                currentRecords
                                    .FirstOrDefault(
                                        x =>
                                            x.Id ==
                                            recordDto
                                                .Id
                                                .Value);

                            if (existing is not null)
                            {
                                existing.Diagnosis =
                                    recordDto
                                        .Diagnosis;

                                existing.Notes =
                                    recordDto
                                        .Notes;

                                medicalRepo
                                    .Update(existing);
                            }
                        }
                        else
                        {
                            // Add new
                            await medicalRepo
                                .AddAsync(
                                    new MedicalRecord
                                    {
                                        PatientId =
                                            patient.Id,

                                        Diagnosis =
                                            recordDto
                                                .Diagnosis,

                                        Notes =
                                            recordDto
                                                .Notes,

                                        CreatedAt =
                                            DateTime
                                                .UtcNow
                                    });
                        }
                    }
                }

                await _unitOfWork
                    .SaveChangesAsync();
            }
            catch
            {
                // 🔥 Rollback Identity
                await _identityClient
                    .UpdatePatientAsync(
                        patient.IdentityUserId
                            .ToString(),
                        new UpdateIdentityUserRequest
                        {
                            DisplayName =
                                oldIdentityData
                                    .Value
                                    .DisplayName,

                            Email =
                                oldIdentityData
                                    .Value
                                    .Email,

                            PhoneNumber =
                                oldIdentityData
                                    .Value
                                    .PhoneNumber
                        },
                        GetToken());

                throw;
            }
        }

        #endregion

        #region ❌ Delete

        public async Task DeleteAsync(Guid id)
        {
            var repo =
                _unitOfWork.GetRepository<Patient, Guid>();

            var patient =
                await repo.GetByIdAsync(id);

            if (patient is null)
                throw new KeyNotFoundException("Patient not found");

            // ✅ Owner only
            if (patient.IdentityUserId != GetCurrentUserId())
                throw new UnauthorizedAccessException();

            // =========================
            // Delete From Identity FIRST
            // =========================

            var identityResult =
                await _identityClient.DeleteUserAsync(
                    patient.IdentityUserId.ToString(),
                    GetToken());

            if (identityResult.IsFailure)
                throw new Exception(
                    identityResult.Errors.First().Description);

            try
            {
                // =========================
                // Delete Patient DB
                // =========================

                repo.Remove(patient);

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // ⚠️ للأسف مفيش rollback سهل
                // لأن اليوزر اتحذف من Identity بالفعل

                throw new Exception(
                    "Patient deleted from Identity but failed in Patient DB");
            }
        }

        #endregion

        #region ❌ Delete MedicalRecord

        public async Task DeleteMedicalRecordAsync(
            int medicalRecordId)
        {
            var identityUserId =
                GetCurrentUserId();

            var patientRepo =
                _unitOfWork.GetRepository<Patient, Guid>();

            var medicalRepo =
                _unitOfWork.GetRepository<MedicalRecord, int>();

            // ✅ هات المريض الحالي
            var patient =
                await patientRepo.GetByIdAsync(
                    new PatientByUserIdSpecification(
                        identityUserId));

            if (patient is null)
                throw new KeyNotFoundException(
                    "Patient profile not found");

            // ✅ هات الـ medical record
            var medicalRecord =
                await medicalRepo.GetByIdAsync(
                    medicalRecordId);

            if (medicalRecord is null)
                throw new KeyNotFoundException(
                    "Medical record not found");

            // ✅ Ownership check
            if (medicalRecord.PatientId != patient.Id)
                throw new UnauthorizedAccessException(
                    "You cannot delete this medical record");

            medicalRepo.Remove(medicalRecord);

            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region ❌ Delete Allergy

        public async Task DeleteAllergyAsync(
            int allergyId)
        {
            var identityUserId =
                GetCurrentUserId();

            var patientRepo =
                _unitOfWork.GetRepository<Patient, Guid>();

            var allergyRepo =
                _unitOfWork.GetRepository<Allergy, int>();

            // ✅ هات المريض الحالي
            var patient =
                await patientRepo.GetByIdAsync(
                    new PatientByUserIdSpecification(
                        identityUserId));

            if (patient is null)
                throw new KeyNotFoundException(
                    "Patient profile not found");

            // ✅ هات allergy
            var allergy =
                await allergyRepo.GetByIdAsync(
                    allergyId);

            if (allergy is null)
                throw new KeyNotFoundException(
                    "Allergy not found");

            // ✅ Ownership check
            if (allergy.PatientId != patient.Id)
                throw new UnauthorizedAccessException(
                    "You cannot delete this allergy");

            allergyRepo.Remove(allergy);

            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region 📊 Get All (Pagination)

        public async Task<PaginationResponse<PatientDto>> GetAllAsync(PatientQueryParams param)
        {
            var repo = _unitOfWork.GetRepository<Patient, Guid>();

            var spec = new PatientSpecification(param);
            var countSpec = new PatientCountSpecification(param);

            var data = await repo.GetAllAsync(spec);
            var count = await repo.CountAsync(countSpec);

            return new PaginationResponse<PatientDto>
            {
                PageIndex = param.PageIndex,
                PageSize = param.PageSize,
                TotalCount = count,
                Data = data.Select(MapToDto).ToList()
            };
        }

        #endregion

        #region 🔥 Get Patient Details By IdentityUserId

        public async Task<ReturnedPatientDetailsDto>
            GetPatientDetailsByIdentityUserIdAsync(
                Guid identityUserId)
        {
            var patientRepo =
                _unitOfWork.GetRepository<Patient, Guid>();

            var medicalRepo =
                _unitOfWork.GetRepository<MedicalRecord, int>();

            var allergyRepo =
                _unitOfWork.GetRepository<Allergy, int>();

            var patient =
                await patientRepo.GetByIdAsync(
                    new PatientByUserIdSpecification(
                        identityUserId.ToString()));

            if (patient is null)
                throw new KeyNotFoundException(
                    "Patient not found");

            var medicalRecords =
                await medicalRepo.GetAllAsync(
                    new MedicalRecordByPatientSpec(
                        patient.Id));

            var allergies =
                await allergyRepo.GetAllAsync(
                    new AllergyByPatientSpec(
                        patient.Id));

            return new ReturnedPatientDetailsDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender.ToString(),
                IdentityUserId = patient.IdentityUserId.ToString(),

                MedicalRecords =
                         medicalRecords
                         .Select(MapMedicalRecordToDto)
                         .ToList(),

                Allergies =
                         allergies
                         .Select(MapAllergyToDto)
                         .ToList()
            };
        }

        #endregion

        #region 🔄 Mapping

        private PatientDto MapToDto(Patient p)
        {
            return new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender.ToString(),
                IdentityUserId = p.IdentityUserId
            };
        }

        private MedicalRecordDto MapMedicalRecordToDto(MedicalRecord r)
        {
            return new MedicalRecordDto
            {
                Id = r.Id,
                PatientId = r.PatientId,
                Diagnosis = r.Diagnosis,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            };
        }

        private AllergyDto MapAllergyToDto(Allergy a)
        {
            return new AllergyDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                Name = a.Name,
                Description = a.Description
            };
        }

        #endregion
    }
}