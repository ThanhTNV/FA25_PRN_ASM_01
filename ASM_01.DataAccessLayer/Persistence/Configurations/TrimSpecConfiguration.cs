using ASM_01.DataAccessLayer.Entities.VehicleModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;

public class TrimSpecConfiguration : IEntityTypeConfiguration<TrimSpec>
{
    public void Configure(EntityTypeBuilder<TrimSpec> builder)
    {
        builder.ToTable("TrimSpec");

        builder.HasKey(x => new { x.EvTrimId, x.SpecId });

        builder.Property(x => x.Value)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne(x => x.EvTrim)
            .WithMany(v => v.TrimSpecs)
            .HasForeignKey(x => x.EvTrimId);

        builder.HasOne(x => x.Spec)
            .WithMany(f => f.TrimSpecs)
            .HasForeignKey(x => x.SpecId);
    }
}
