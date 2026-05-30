using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Domain.Entities
{ 
    public class Patient : BaseEntity<Guid>
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string Address { get; set; } = default!;

        [Required(ErrorMessage = "UserId is required")]
        public string IdentityUserId { get; set; } = default!;

        public ICollection<Allergy> Allergies { get; set; }
            = new List<Allergy>();

        public ICollection<MedicalRecord> MedicalRecords { get; set; }
            = new List<MedicalRecord>();
    }
}
