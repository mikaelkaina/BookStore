using BookStore.Domain.Common;
using FluentAssertions;

namespace BookStore.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Success_SloudlCreateSucessResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = new Error("Test.Error", "Something went wrong");
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessOfT_ShouldExposeValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FailureOfT_AccessingValue_ShouldThrow()
    {
        var result = Result.Failure<int>(new Error("Err", "fail"));

        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateSuccessResult()
    {
        Result<string> result = "hello";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void Success_WithError_ShouldThrow()
    {
        var act = () => new ResultExposed(true, new Error("X", "Y"));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_WithNoError_ShouldThrow()
    {
        var act = () => new ResultExposed(false, Error.None);
        act.Should().Throw<InvalidOperationException>();
    }

    // Helper para testar o construtor protegido
    private class ResultExposed : Result
    {
        public ResultExposed(bool isSuccess, Error error) : base(isSuccess, error) { }
    }
}
