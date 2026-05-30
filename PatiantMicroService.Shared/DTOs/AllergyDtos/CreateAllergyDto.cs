using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.AllergyDtos
{
    public class CreateAllergyDto
    {
        public Guid PatientId { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}
