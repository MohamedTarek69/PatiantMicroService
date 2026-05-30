using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class AddPatientMedicalDataDto
    {
        public ICollection<CreateAllergyDto>? Allergies
        { get; set; }

        public ICollection<CreateMedicalRecordDto>? MedicalRecords
        { get; set; }
    }
}
