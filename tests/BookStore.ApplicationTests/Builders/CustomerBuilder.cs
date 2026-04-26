using BookStore.Domain.Entities;

namespace BookStore.ApplicationTests.Builders;

public class CustomerBuilder
{
    public Customer Build()
    {
        var result = Customer.Create(
            "João",
            "Silva",
            "joao@email.com",
            null,
            "12345678909",
            null
        );

        if (result.IsFailure)
            throw new Exception("CustomerBuilder falhou");

        return result.Value;
    }
}