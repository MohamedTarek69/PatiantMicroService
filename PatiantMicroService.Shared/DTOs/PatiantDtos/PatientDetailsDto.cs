using PatiantMicroService.Shared.DTOs.AllergyDtos;
using PatiantMicroService.Shared.DTOs.MedicalRecordDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class PatientDetailsDto
    {
        //public PatientDto Patient { get; set; } = default!;

        public List<MedicalRecordDto> MedicalRecords { get; set; } = [];

        public List<AllergyDto> Allergies { get; set; } = [];
    }
}
