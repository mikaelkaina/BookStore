namespace BookStore.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    PaymentConfirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Returned = 7
}
