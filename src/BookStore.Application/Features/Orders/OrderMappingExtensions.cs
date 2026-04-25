using BookStore.Application.Features.Orders.Commands.CreateOrder;
using BookStore.Domain.Entities;

namespace BookStore.Application.Features.Orders;

public static class OrderMappingExtensions
{
    public static CreateOrderResponse ToCreateOrderResponse(this Order order) =>
        new(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Status.ToString(),
            order.SubTotal.Amount,
            order.ShippingCost.Amount,
            order.Discount.Amount,
            order.Total.Amount,
            order.Total.Currency,
            order.Items.Select(i => i.ToIntemResponse()),
            order.CreatedAt
        );

    private static OrderItemResponse ToIntemResponse(this OrderItem item) =>
        new(
            item.BookId,
            item.BookTitle,
            item.UnitPrice.Amount,
            item.Quantity,
            item.TotalPrice.Amount,
            item.TotalPrice.Currency
        );
}
