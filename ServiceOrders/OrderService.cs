namespace ServiceOrders;

// Модуль M4: заявки на установку оборудования
public class OrderService
{
    private readonly EquipmentService equipment;

    public OrderService(EquipmentService equipment)
    {
        this.equipment = equipment;
    }

    public decimal CalculateTotal(Order order, decimal discountPercent = 0)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(nameof(discountPercent));
        decimal total = 0;
        foreach (OrderItem item in order.Items)
        {
            if (item.Quantity <= 0)
                throw new ArgumentException("Количество должно быть больше нуля");
            total += item.Price * item.Quantity;
        }
        return Math.Round(total * (1 - discountPercent / 100m), 2);
    }

    public void ChangeStatus(Order order, OrderStatus newStatus)
    {
        bool allowed = (order.Status, newStatus) switch
        {
            (OrderStatus.New, OrderStatus.Planned) => true,
            (OrderStatus.Planned, OrderStatus.InProgress) => true,
            (OrderStatus.InProgress, OrderStatus.Done) => true,
            (OrderStatus.New, OrderStatus.Cancelled) => true,
            (OrderStatus.Planned, OrderStatus.Cancelled) => true,
            _ => false
        };
        if (!allowed)
            throw new InvalidOperationException($"Переход {order.Status} -> {newStatus} недопустим");
        order.Status = newStatus;
    }

    public void Complete(Order order)
    {
        if (order.Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Заявка должна быть в статусе InProgress");

        // сначала проверяем наличие всего оборудования, потом списываем
        foreach (OrderItem item in order.Items)
        {
            if (equipment.GetStock(item.EquipmentId) < item.Quantity)
                throw new InvalidOperationException($"Недостаточно оборудования: {item.Name}");
        }
        foreach (OrderItem item in order.Items)
            equipment.WriteOff(item.EquipmentId, item.Quantity);

        ChangeStatus(order, OrderStatus.Done);
    }

    // Модуль M5 (упрощённо): текст акта выполненных работ
    public string BuildAct(Order order, Client client, decimal total)
    {
        var lines = new List<string>
        {
            $"АКТ выполненных работ по заявке №{order.Id}",
            $"Клиент: {client.Name}",
            "Выполнено:"
        };
        foreach (OrderItem item in order.Items)
            lines.Add($"  {item.Name} - {item.Quantity} шт. x {item.Price} руб.");
        lines.Add($"Скидка: {client.DiscountPercent}%");
        lines.Add($"Итого к оплате: {total} руб.");
        return string.Join(Environment.NewLine, lines);
    }
}
