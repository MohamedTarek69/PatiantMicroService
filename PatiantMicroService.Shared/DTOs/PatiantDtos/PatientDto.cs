using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class PatientDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = default!;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string IdentityUserId { get; set; } = default!;
    }
}
