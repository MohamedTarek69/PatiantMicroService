using PatiantMicroService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Services.Specifications
{
    public class MedicalRecordByPatientSpec
    : BaseSpecification<MedicalRecord, int>
    {
        public MedicalRecordByPatientSpec(Guid patientId)
            : base(r => r.PatientId == patientId)
        {
        }
    }
}
