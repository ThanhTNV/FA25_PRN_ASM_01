using ASM_01.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;

public class TrimPriceConfiguration : IEntityTypeConfiguration<TrimPrice>
{
    public void Configure(EntityTypeBuilder<TrimPrice> builder)
    {
        builder.ToTable("TrimPrice");

        builder.HasKey(x => x.TrimPriceId);

        builder.Property(x => x.ListedPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.EffectiveDate)
            .IsRequired();

        builder.HasOne(x => x.EvTrim)
            .WithMany(v => v.Prices)
            .HasForeignKey(x => x.EvTrimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}