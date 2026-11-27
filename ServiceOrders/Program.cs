using ServiceOrders;

Console.OutputEncoding = System.Text.Encoding.UTF8;

const int Terminal = 1;
const int FuelSensor = 2;

var equipment = new EquipmentService();
var orders = new OrderService(equipment);

equipment.AddStock(Terminal, 5);
equipment.AddStock(FuelSensor, 3);
Console.WriteLine($"Остаток на складе: терминалы {equipment.GetStock(Terminal)}, датчики топлива {equipment.GetStock(FuelSensor)}");

var client = new Client { Id = 1, Name = "ООО Транс-Групп", Inn = "7701234567", Phone = "+79990001122", DiscountPercent = 10 };
Console.WriteLine($"ИНН клиента корректен: {ClientValidator.IsValidInn(client.Inn)}");
Console.WriteLine($"ИНН '12345abc90' корректен: {ClientValidator.IsValidInn("12345abc90")}");

// Заявка 1: терминал - 1 шт., датчик топлива - 2 шт.
var order = new Order { Id = 1, ClientId = client.Id, VehicleId = 1 };
order.Items.Add(new OrderItem { EquipmentId = Terminal, Name = "Терминал ГЛОНАСС", Price = 5000, Quantity = 1 });
order.Items.Add(new OrderItem { EquipmentId = FuelSensor, Name = "Датчик уровня топлива", Price = 3000, Quantity = 2 });

Console.WriteLine($"Сумма без скидки: {orders.CalculateTotal(order)} руб.");
decimal total = orders.CalculateTotal(order, client.DiscountPercent);
Console.WriteLine($"Сумма со скидкой {client.DiscountPercent}%: {total} руб.");

orders.ChangeStatus(order, OrderStatus.Planned);
orders.ChangeStatus(order, OrderStatus.InProgress);
orders.Complete(order);
Console.WriteLine($"Статус заявки 1: {order.Status}");
Console.WriteLine($"Остаток после выполнения: терминалы {equipment.GetStock(Terminal)}, датчики топлива {equipment.GetStock(FuelSensor)}");
Console.WriteLine(orders.BuildAct(order, client, total));

// Заявка 2: оборудования не хватает
var order2 = new Order { Id = 2, ClientId = client.Id, VehicleId = 2 };
order2.Items.Add(new OrderItem { EquipmentId = FuelSensor, Name = "Датчик уровня топлива", Price = 3000, Quantity = 10 });
orders.ChangeStatus(order2, OrderStatus.Planned);
orders.ChangeStatus(order2, OrderStatus.InProgress);
try
{
    orders.Complete(order2);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("Ошибка: " + ex.Message);
}
Console.WriteLine($"Статус заявки 2: {order2.Status}, остаток датчиков: {equipment.GetStock(FuelSensor)}");

// Недопустимый переход статуса
try
{
    orders.ChangeStatus(order, OrderStatus.New);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("Ошибка: " + ex.Message);
}
