using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class UpdateAllergyDto
    {
        public int? Id { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}
