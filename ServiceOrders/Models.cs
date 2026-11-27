namespace ServiceOrders;

public enum OrderStatus { New, Planned, InProgress, Done, Cancelled }

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Inn { get; set; } = "";
    public string Phone { get; set; } = "";
    public decimal DiscountPercent { get; set; }
}

public class OrderItem
{
    public int EquipmentId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int VehicleId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public List<OrderItem> Items { get; } = new();
}
