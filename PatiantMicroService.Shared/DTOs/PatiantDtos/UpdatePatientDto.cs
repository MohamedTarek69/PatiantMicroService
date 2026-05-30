using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class UpdatePatientDto
    {
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        // ✅ Nested Updates
        public ICollection<UpdateAllergyDto>? Allergies { get; set; }

        public ICollection<UpdateMedicalRecordDto>? MedicalRecords { get; set; }
    }
}
