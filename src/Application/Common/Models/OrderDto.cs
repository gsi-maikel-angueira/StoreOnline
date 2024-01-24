namespace StoreOnline.Application.Common.Models;

public class OrderDto
{
   public int? CustomerId { get; set; }
   public List<ProductDto> Products { get; set; } = new();
}
