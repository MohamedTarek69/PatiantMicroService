using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Shared.DTOs.PatiantDtos
{
    public class PatientQueryParams
    {
        public string? Search { get; set; }

        public string? IdentityUserId { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Sort { get; set; }
    }
}
