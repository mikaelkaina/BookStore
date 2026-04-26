using BookStore.Application.Features.Orders.Commands.CreateOrder;
using BookStore.Application.Features.Orders.Queries.GetOrderById;
using BookStore.Application.Features.Orders.Queries.GetOrdersByCustomer;
using BookStore.Application.Features.Orders.Queries.GetOrdersPaged;
using BookStore.Application.Features.Orders.Shared;
using BookStore.Domain.Entities;
using BookStore.Domain.ValueObjects;

namespace BookStore.Application.Features.Orders;

public static class OrderMappingExtensions
{
    public static CreateOrderResponse ToCreateResponse(this Order order) =>
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
            order.Items.Select(i => i.ToItemResponse()),
            order.CreatedAt);

    public static GetOrderByIdResponse ToGetByIdResponse(this Order order) =>
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
            order.ShippingAddress.ToAddressResponse(),
            order.Items.Select(i => i.ToItemResponse()),
            order.Notes,
            order.ShippedAt,
            order.DeliveredAt,
            order.CancelledAt,
            order.CreatedAt,
            order.UpdatedAt);

    public static GetOrdersByCustomerResponse ToGetByCustomerResponse(this Order order) =>
        new(
            order.Id,
            order.OrderNumber,
            order.Status.ToString(),
            order.Total.Amount,
            order.Total.Currency,
            order.Items.Count,
            order.CreatedAt);

    public static GetOrdersPagedResponse ToGetPagedResponse(this Order order) =>
     new(
         order.Id,
         order.OrderNumber,
         order.CustomerId,
         order.Status.ToString(),
         order.Total.Amount,
         order.Total.Currency,
         order.Items.Count,
         order.CreatedAt);

    private static ShippingAddressResponse ToAddressResponse(this Address address) =>
        new(
            address.Street,
            address.Number,
            address.Complement,
            address.Neighborhood,
            address.City,
            address.State,
            address.FormattedZipCode);

    private static OrderItemResponse ToItemResponse(this OrderItem item) =>
        new(
            item.BookId,
            item.BookTitle,
            item.UnitPrice.Amount,
            item.Quantity,
            item.TotalPrice.Amount,
            item.UnitPrice.Currency);

}
