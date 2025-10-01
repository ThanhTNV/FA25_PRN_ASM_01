namespace ASM_01.BusinessLayer.DTOs;

public record ModelStockDto
{
    public string ModelName { get; set; } = null!;
    public int TotalQuantity { get; set; }
    public List<TrimQty> Trims { get; set; } = new();
}

public record TrimQty
{
    public int EvTrimId { get; set; }
    public string TrimName { get; set; } = null!;
    public int Quantity { get; set; }
}