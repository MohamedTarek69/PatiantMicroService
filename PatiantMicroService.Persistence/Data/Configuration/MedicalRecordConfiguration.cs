using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatiantMicroService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatiantMicroService.Persistence.Data.Configuration
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Diagnosis)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.Notes)
                .HasMaxLength(1000);

            builder.Property(m => m.CreatedAt)
                .IsRequired();
        }
    }
}
