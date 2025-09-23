using ASM_01.DataAccessLayer.Entities;
using ASM_01.DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;

public class EvModelConfiguration : IEntityTypeConfiguration<EvModel>
{
    public void Configure(EntityTypeBuilder<EvModel> builder)
    {
        builder.ToTable("EvModel");

        builder.HasKey(x => x.EvModelId);

        builder.Property(x => x.ModelName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnType("text");

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();
    }
}
