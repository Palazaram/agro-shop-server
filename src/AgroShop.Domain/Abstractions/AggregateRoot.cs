using CSharpFunctionalExtensions;

namespace AgroShop.Domain.Abstractions;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : IComparable<TId>
{
    protected AggregateRoot(TId id) : base(id) { }
    protected AggregateRoot() { }
}