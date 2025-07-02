namespace SpireCore.Abstractions.Interfaces;

public interface IHasId
{
    string Id { get; }
}

public interface IHasId<T>
{
    T Id { get; set; }
}

