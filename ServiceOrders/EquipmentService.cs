namespace ServiceOrders;

// Модуль M3: учёт оборудования на складе
public class EquipmentService
{
    private readonly Dictionary<int, int> stock = new();

    public void AddStock(int equipmentId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля", nameof(quantity));
        stock[equipmentId] = GetStock(equipmentId) + quantity;
    }

    public int GetStock(int equipmentId)
    {
        return stock.TryGetValue(equipmentId, out int value) ? value : 0;
    }

    public void WriteOff(int equipmentId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля", nameof(quantity));
        if (!stock.TryGetValue(equipmentId, out int available))
            throw new KeyNotFoundException("Оборудование не найдено на складе");
        if (available < quantity)
            throw new InvalidOperationException(
                $"Недостаточно оборудования: на складе {available}, требуется {quantity}");
        stock[equipmentId] = available - quantity;
    }
}
