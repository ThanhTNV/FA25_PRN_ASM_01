using ASM_01.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;
public class EvTrimConfiguration : IEntityTypeConfiguration<EvTrim>
{
    public void Configure(EntityTypeBuilder<EvTrim> builder)
    {
        builder.ToTable("EvTrim");

        builder.HasKey(x => x.EvTrimId);

        builder.Property(x => x.TrimName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnType("text");

        builder.HasOne(x => x.EvModel)
            .WithMany(m => m.Trims)
            .HasForeignKey(x => x.EvModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}