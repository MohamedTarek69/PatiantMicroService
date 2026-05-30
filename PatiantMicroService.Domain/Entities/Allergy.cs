using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Domain.Entities
{
    public class Allergy : BaseEntity<int>
    {
        [Required]
        public Guid PatientId { get; set; }

        public Patient Patient { get; set; } = default!;

        [Required(ErrorMessage = "Allergy name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(500)]
        public string Description { get; set; } = default!;
    }
}
