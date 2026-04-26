using BookStore.Application.Features.Carts.Commands.AddItemToCart;
using BookStore.Application.Features.Carts.Queries.GetCart;
using BookStore.Application.Features.Carts.Shared;
using BookStore.Domain.Entities;

namespace BookStore.Application.Features.Carts;

public static class CartMappingExtensions
{
    public static AddItemToCartResponse ToAddItemResponse(this Cart cart) =>
        new(
            cart.Id,
            cart.CustomerId,
            cart.SessionId,
            cart.Items.Select(i => i.ToItemResponse()),
            cart.Total.Amount,
            cart.Total.Currency,
            cart.TotalItems,
            cart.ExpiresAt
        );

    public static GetCartResponse ToGetCartResponse(this Cart cart) =>
        new(
            cart.Id,
            cart.CustomerId,
            cart.SessionId,
            cart.Items.Select(i => i.ToItemResponse()),
            cart.Total.Amount,
            cart.Total.Currency,
            cart.TotalItems,
            cart.IsCheckedOut,
            cart.ExpiresAt,
            cart.CreatedAt,
            cart.UpdatedAt
        );

    private static CartItemResponse ToItemResponse(this CartItem item) =>
       new(
           item.BookId,
           item.BookTitle,
           item.BookCoverUrl,
           item.UnitPrice.Amount,
           item.Quantity,
           item.TotalPrice.Amount,
           item.UnitPrice.Currency
       );
}
