using PatiantMicroService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Services.Specifications
{
    public class AllergyByPatientSpec
    : BaseSpecification<Allergy, int>
    {
        public AllergyByPatientSpec(Guid patientId)
            : base(a => a.PatientId == patientId)
        {
        }
    }
}
