using ASM_01.DataAccessLayer.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASM_01.DataAccessLayer.Persistence.Configurations;

public class DistributionRequestConfiguration : IEntityTypeConfiguration<DistributionRequest>
{
    public void Configure(EntityTypeBuilder<DistributionRequest> builder)
    {
        builder.HasKey(dr => dr.DistributionRequestId);

        builder.Property(dr => dr.RequestedQuantity)
            .IsRequired();

        builder.Property(dr => dr.RequestedAt)
            .IsRequired();
        
        builder.Property(dr => dr.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(dr => dr.Dealer)
            .WithMany(d => d.DistributionRequests)
            .HasForeignKey(dr => dr.DealerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dr => dr.EvTrim)
            .WithMany(et => et.DistributionRequests)
            .HasForeignKey(dr => dr.EvTrimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
/*
 public int DistributionRequestId { get; set; }
    public int DealerId { get; set; }
    public int EvTrimId { get; set; }
    public int RequestedQuantity { get; set; }
    public int? ApprovedQuantity { get; set; }
    public DistributionStatus Status { get; set; } = DistributionStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
 */