using ReactiveOrders.Contracts.Commands;

namespace ReactiveOrders.Domain;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string AttendeeId { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Placed;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? EstimatedReady { get; private set; }
    public string? RejectionReason { get; private set; }
    public List<OrderLine> Lines { get; private set; } = [];

    private Order() { }

    public static Order Create(PlaceOrder command)
    {
        var order = new Order
        {
            AttendeeId = command.AttendeeId,
            Notes = command.Notes,
            Lines = command.Items.Select(i => new OrderLine
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
        return order;
    }

    public void Confirm(DateTime estimatedReady)
    {
        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        EstimatedReady = estimatedReady;
    }

    public void Reject(string reason)
    {
        Status = OrderStatus.Rejected;
        RejectionReason = reason;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
    }

    public decimal TotalAmount => Lines.Sum(l => l.Quantity * l.UnitPrice);
}

public class OrderLine
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public enum OrderStatus
{
    Placed,
    Confirmed,
    Rejected,
    Cancelled
}
