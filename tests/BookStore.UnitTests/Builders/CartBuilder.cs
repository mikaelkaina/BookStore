using BookStore.Domain.Entities;

namespace BookStore.UnitTests.Builders;

public class CartBuilder
{
    private bool _forGuest = false;
    private Guid _customerId = Guid.NewGuid();
    private string _sessionId = Guid.NewGuid().ToString();

    public CartBuilder ForGuest() { _forGuest = true; return this; }
    public CartBuilder ForGuest(string sessionId) { _forGuest = true; _sessionId = sessionId; return this; }
    public CartBuilder WithCustomerId(Guid customerId) { _customerId = customerId; return this; }

    public Cart Build() => _forGuest
        ? Cart.CreateForGuest(_sessionId)
        : Cart.CreateForCustomer(_customerId);
}