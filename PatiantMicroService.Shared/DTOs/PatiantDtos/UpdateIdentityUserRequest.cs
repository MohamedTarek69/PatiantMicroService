using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class UpdateIdentityUserRequest
    {
        public string DisplayName { get; set; } = default!;
        
        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;
    }
}
