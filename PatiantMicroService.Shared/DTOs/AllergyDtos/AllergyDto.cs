using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.AllergyDtos
{
    public class AllergyDto
    {
        public int Id { get; set; }

        public Guid PatientId { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}
