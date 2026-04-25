using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.StartOrderProcessing;

public sealed record StartOrderProcessingCommand(Guid OrderId) : ICommand<Result>;
