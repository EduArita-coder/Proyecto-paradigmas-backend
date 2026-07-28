namespace GAMEHOSTING_APIREST.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Cpu { get; set; } = string.Empty;
    public string Ram { get; set; } = string.Empty;
    public int Slots { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
