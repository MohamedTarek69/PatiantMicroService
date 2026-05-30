using PatiantMicroService.Shared.DTOs.AllergyDtos;
using PatiantMicroService.Shared.DTOs.MedicalRecordDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class ReturnedPatientDetailsDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = default!;

        public string Address { get; set; } = default!;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = default!;

        public string IdentityUserId { get; set; } = default!;

        public ICollection<MedicalRecordDto> MedicalRecords
        { get; set; } = [];

        public ICollection<AllergyDto> Allergies
        { get; set; } = [];
    }
}
