using PatiantMicroService.Domain.Entities;
using PatiantMicroService.Shared.DTOs.PatiantDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Services.Specifications
{
    public class PatientCountSpecification
     : BaseSpecification<Patient, Guid>
    {
        public PatientCountSpecification(PatientQueryParams param)
            : base(p =>
                (string.IsNullOrEmpty(param.Search) ||
                 p.FullName.ToLower().Contains(param.Search.ToLower())) &&

                (string.IsNullOrEmpty(param.IdentityUserId) || p.IdentityUserId == param.IdentityUserId)
            )
            {
            }
    }
}
