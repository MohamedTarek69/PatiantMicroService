using PatiantMicroService.Shared.CommonResult;
using PatiantMicroService.Shared.DTOs.PatiantDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.ServicesAbstraction.Interfaces
{
    public interface IPatientService
    {
        Task<PatientProfileDto> GetMyProfileAsync();

        Task<PatientDetailsDto> GetMyDetailsAsync();

        Task<Guid> CreateAsync(CreatePatientDto dto);

        Task AddMedicalDataAsync(AddPatientMedicalDataDto dto);

        Task UpdateAsync(UpdatePatientDto dto);

        Task DeleteAsync(Guid id);

        Task<PaginationResponse<PatientDto>> GetAllAsync(PatientQueryParams param);

        Task<ReturnedPatientDetailsDto> GetPatientDetailsByIdentityUserIdAsync(Guid identityUserId);

        Task DeleteMedicalRecordAsync(int medicalRecordId);

        Task DeleteAllergyAsync(int allergyId);
    }
}
