using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.MedicalRecordDtos
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }

        public Guid PatientId { get; set; }

        public string Diagnosis { get; set; } = default!;

        public string Notes { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }
}
