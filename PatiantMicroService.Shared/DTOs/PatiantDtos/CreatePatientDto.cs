using PatiantMicroService.Shared.DTOs.AllergyDtos;
using PatiantMicroService.Shared.DTOs.MedicalRecordDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class CreatePatientDto
    {
        //public string? FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public GenderDto Gender { get; set; }

        public string Address { get; set; } = default!;

        public Guid UserId { get; set; }

        // ✅ Nested Data
        public ICollection<CreateAllergyDto>? Allergies { get; set; }

        public ICollection<CreateMedicalRecordDto>? MedicalRecords { get; set; }
    }
}
