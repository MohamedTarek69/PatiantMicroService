using PatiantMicroService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Services.Specifications
{
    public class PatientByUserIdSpecification
     : BaseSpecification<Patient, Guid>
    {
        public PatientByUserIdSpecification(string userId)
            : base(p => p.IdentityUserId == userId)
        {
        }
    }
}
