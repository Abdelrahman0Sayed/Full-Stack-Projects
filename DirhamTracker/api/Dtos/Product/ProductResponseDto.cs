namespace api.Dtos.Product;

public class ProductResponseDto
{
    public string? Message { get; set; }
    
    public string? ProductId { get; set; }
    
    public string? Category { get; set; }
    
    public string? CardNumber { get; set; }
    
    public float Paid { get; set; }
    
    public DateTime? PaymentTime { get; set; }

}