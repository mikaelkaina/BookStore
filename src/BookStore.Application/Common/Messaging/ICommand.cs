using BookStore.Domain.Common;
using MediatR;

namespace BookStore.Application.Common.Messaging;

public interface ICommand : IRequest<Result> { }

public interface ICommand<TResponse> : IRequest<TResponse> { }

public interface IQuery<TResponse> : IRequest<TResponse> { }
