using AgroShop.Domain.Errors;
using CSharpFunctionalExtensions;

namespace AgroShop.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResponse>
{
    Task<Result<TResponse, Error>> HandleAsync(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<in TCommand>
{
    Task<UnitResult<Error>> HandleAsync(TCommand command, CancellationToken ct);
}