using ASM_01.DataAccessLayer.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;

public class VehicleStockConfiguration : IEntityTypeConfiguration<VehicleStock>
{
    public void Configure(EntityTypeBuilder<VehicleStock> builder)
    {
        builder.HasKey(vs => vs.VehicleStockId);
        builder.Property(vs => vs.Quantity)
            .IsRequired();
        
        builder.HasOne(vs => vs.Dealer)
            .WithMany(d => d.VehicleStocks)
            .HasForeignKey(vs => vs.DealerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vs => vs.EvTrim)
            .WithMany(et => et.VehicleStocks)
            .HasForeignKey(vs => vs.EvTrimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
