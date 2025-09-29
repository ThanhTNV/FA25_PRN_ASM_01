
namespace ASM_01.WebApp.Models
{
    public class PendingRequestViewModel
    {
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ApprovedQuantity { get; set; }
        public int RequestQuantity { get; set; }
        public int ModelYear { get; set; }
        public string TrimName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public int RequestId { get; set; }
    }
}
