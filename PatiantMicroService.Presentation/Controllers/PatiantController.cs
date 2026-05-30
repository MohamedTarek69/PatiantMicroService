using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatiantMicroService.ServicesAbstraction.Interfaces;
using PatiantMicroService.Shared.CommonResult;
using PatiantMicroService.Shared.DTOs.PatiantDtos;

namespace PatiantMicroService.Presentation.Controllers
{
    [ApiController]
    [Route("Patiant")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PatiantController : ApiBaseController
    {
        private readonly IPatientService _patientService;

        public PatiantController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        #region 🧍 My Profile

        // ✅ Patient فقط
        [Authorize(Roles = "Patient")]
        [HttpGet("MyProfile")]
        public async Task<ActionResult<PatientProfileDto>> GetMyProfile()
        {
            var result = await _patientService.GetMyProfileAsync();

            return Ok(result);
        }

        #endregion

        #region 🔥 My Details

        // ✅ Patient فقط
        [Authorize(Roles = "Patient")]
        [HttpGet("MyDetails")]
        public async Task<ActionResult<PatientDetailsDto>> GetMyDetails()
        {
            var result = await _patientService.GetMyDetailsAsync();

            return Ok(result);
        }

        #endregion

        #region ➕ Create Profile

        // ✅ Patient فقط
        [Authorize(Roles = "Patient")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            [FromBody] CreatePatientDto dto)
        {
            if (!ModelState.IsValid)
                return HandleModelStateErrors(ModelState);

            var result = await _patientService.CreateAsync(dto);

            return Ok(result);
        }

        #endregion

        #region ✏️ Update

        // ✅ Patient أو Admin
        [Authorize(Roles = "AdminOrPatiant")]
        [HttpPut("Update/{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdatePatientDto dto)
        {
            if (!ModelState.IsValid)
                return HandleModelStateErrors(ModelState);

            await _patientService.UpdateAsync(id, dto);

            return NoContent();
        }

        #endregion

        #region ❌ Delete

        // ✅ Admin فقط
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _patientService.DeleteAsync(id);

            return NoContent();
        }

        #endregion

        #region 📊 Get All

        // ✅ Admin فقط
        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public async Task<ActionResult<PaginationResponse<PatientDto>>> GetAll(
            [FromQuery] PatientQueryParams param)
        {
            var result = await _patientService.GetAllAsync(param);

            return Ok(result);
        }

        #endregion

        #region ➕ Add MedicalRecords & Allergies

        // ✅ Patient فقط
        [Authorize(Roles = "Patient")]
        [HttpPost("MyMedicalData")]
        public async Task<IActionResult> AddMedicalData([FromBody] AddPatientMedicalDataDto dto)
        {
            if (!ModelState.IsValid)
                return HandleModelStateErrors(ModelState);

            await _patientService.AddMedicalDataAsync(dto);

            return Ok(new
            {
                success = true,
                message = "Medical data added successfully"
            });
        }

        #endregion

        #region 🔥 Get Patient Details By IdentityUserId
        // ✅ Doctor, Admin
        [Authorize(Roles = "AdminOrDoctor")]
        [HttpGet("DetailsByIdentityUserId/{identityUserId:guid}")]
        public async Task<ActionResult<ReturnedPatientDetailsDto>> GetPatientDetailsByIdentityUserId(Guid identityUserId)
        {
            var result = await _patientService.GetPatientDetailsByIdentityUserIdAsync(identityUserId);
            return Ok(result);
        }

        #endregion
    }
}