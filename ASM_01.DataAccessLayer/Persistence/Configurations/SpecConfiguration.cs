using ASM_01.DataAccessLayer.Entities.VehicleModels;
using ASM_01.DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;

public class SpecConfiguration : IEntityTypeConfiguration<Spec>
{
    public void Configure(EntityTypeBuilder<Spec> builder)
    {
        builder.ToTable("Spec");

        builder.HasKey(x => x.SpecId);

        builder.Property(x => x.SpecName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.SpecName).IsUnique();

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Category)
            .HasMaxLength(100);
    }
}
