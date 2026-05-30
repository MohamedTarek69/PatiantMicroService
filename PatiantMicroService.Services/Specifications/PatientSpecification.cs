using PatiantMicroService.Domain.Entities;
using PatiantMicroService.Shared.DTOs.PatiantDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Services.Specifications
{
    public class PatientSpecification
     : BaseSpecification<Patient, Guid>
    {
        public PatientSpecification(PatientQueryParams param)
            : base(p =>
                (string.IsNullOrEmpty(param.Search) ||
                 p.FullName.ToLower().Contains(param.Search.ToLower())) &&

                (string.IsNullOrEmpty(param.IdentityUserId) || p.IdentityUserId == param.IdentityUserId)
            )
        {
            ApplyPagination((param.PageIndex - 1) * param.PageSize, param.PageSize);

            if (!string.IsNullOrEmpty(param.Sort))
            {
                switch (param.Sort)
                {
                    case "nameAsc":
                        AddOrderBy(p => p.FullName);
                        break;

                    case "nameDesc":
                        AddOrderByDescending(p => p.FullName);
                        break;
                }
            }
        }
    }
}
