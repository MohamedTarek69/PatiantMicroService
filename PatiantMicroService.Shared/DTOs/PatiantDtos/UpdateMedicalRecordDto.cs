using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class UpdateMedicalRecordDto
    {
        public int? Id { get; set; }

        public string Diagnosis { get; set; } = default!;

        public string? Notes { get; set; }
    }
}
