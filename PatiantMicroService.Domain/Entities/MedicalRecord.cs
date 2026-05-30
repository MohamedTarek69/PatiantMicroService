using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Domain.Entities
{
    public class MedicalRecord : BaseEntity<int>
    {
        [Required]
        public Guid PatientId { get; set; }

        public Patient Patient { get; set; } = default!;

        [Required(ErrorMessage = "Diagnosis is required")]
        [MaxLength(500)]
        public string Diagnosis { get; set; } = default!;

        [MaxLength(1000)]
        public string Notes { get; set; } = default!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
