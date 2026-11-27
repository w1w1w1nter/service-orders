using ServiceOrders;
using Xunit;

namespace ServiceOrders.Tests;

public class OrderServiceTests
{
    private static Order MakeOrder()
    {
        var order = new Order { Id = 1 };
        order.Items.Add(new OrderItem { EquipmentId = 1, Name = "Терминал", Price = 5000, Quantity = 1 });
        order.Items.Add(new OrderItem { EquipmentId = 2, Name = "Датчик топлива", Price = 3000, Quantity = 2 });
        return order;
    }

    [Theory]
    [InlineData(0, 11000)]
    [InlineData(10, 9900)]
    [InlineData(100, 0)]
    public void CalculateTotal_WithDiscount_ReturnsExpected(double discount, double expected)
    {
        var service = new OrderService(new EquipmentService());
        Assert.Equal((decimal)expected, service.CalculateTotal(MakeOrder(), (decimal)discount));
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(101)]
    public void CalculateTotal_BadDiscount_Throws(double discount)
    {
        var service = new OrderService(new EquipmentService());
        Assert.Throws<ArgumentOutOfRangeException>(() => service.CalculateTotal(MakeOrder(), (decimal)discount));
    }

    [Fact]
    public void ChangeStatus_DoneToNew_Throws()
    {
        var service = new OrderService(new EquipmentService());
        var order = MakeOrder();
        order.Status = OrderStatus.Done;
        Assert.Throws<InvalidOperationException>(() => service.ChangeStatus(order, OrderStatus.New));
    }

    [Fact]
    public void Complete_NotEnoughStock_ThrowsAndStockUnchanged()
    {
        var equipment = new EquipmentService();
        equipment.AddStock(1, 5);
        equipment.AddStock(2, 1);
        var service = new OrderService(equipment);
        var order = MakeOrder();
        order.Status = OrderStatus.InProgress;

        Assert.Throws<InvalidOperationException>(() => service.Complete(order));
        Assert.Equal(5, equipment.GetStock(1));
        Assert.Equal(1, equipment.GetStock(2));
        Assert.Equal(OrderStatus.InProgress, order.Status);
    }

    [Fact]
    public void Complete_Success_WritesOffAndSetsDone()
    {
        var equipment = new EquipmentService();
        equipment.AddStock(1, 5);
        equipment.AddStock(2, 3);
        var service = new OrderService(equipment);
        var order = MakeOrder();
        order.Status = OrderStatus.InProgress;

        service.Complete(order);

        Assert.Equal(4, equipment.GetStock(1));
        Assert.Equal(1, equipment.GetStock(2));
        Assert.Equal(OrderStatus.Done, order.Status);
    }
}

public class EquipmentServiceTests
{
    [Fact]
    public void WriteOff_MoreThanStock_Throws()
    {
        var equipment = new EquipmentService();
        equipment.AddStock(1, 2);
        Assert.Throws<InvalidOperationException>(() => equipment.WriteOff(1, 5));
        Assert.Equal(2, equipment.GetStock(1));
    }

    [Fact]
    public void WriteOff_ZeroQuantity_Throws()
    {
        var equipment = new EquipmentService();
        equipment.AddStock(1, 2);
        Assert.Throws<ArgumentException>(() => equipment.WriteOff(1, 0));
    }

    [Fact]
    public void WriteOff_UnknownItem_Throws()
    {
        var equipment = new EquipmentService();
        Assert.Throws<KeyNotFoundException>(() => equipment.WriteOff(99, 1));
    }
}

public class ClientValidatorTests
{
    [Theory]
    [InlineData("7701234567", true)]
    [InlineData("770123456789", true)]
    [InlineData("123456789", false)]
    [InlineData("12345abc90", false)]
    [InlineData("", false)]
    public void IsValidInn_Cases(string inn, bool expected)
    {
        Assert.Equal(expected, ClientValidator.IsValidInn(inn));
    }

    [Fact]
    public void IsValidVin_Length17_True() => Assert.True(ClientValidator.IsValidVin("XTA210930Y2765432"));

    [Fact]
    public void IsValidVin_Length16_False() => Assert.False(ClientValidator.IsValidVin("XTA210930Y276543"));
}
