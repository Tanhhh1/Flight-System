namespace Application.Interfaces.CQRS
{
    public interface IInvalidateCache
    {
        IEnumerable<string> InvalidatePrefixes { get; }
    }
}
