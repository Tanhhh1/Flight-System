namespace Application.Interfaces.Services
{
    public interface IMemoryCacheService
    {
        bool TryGet<T>(string key, out T? value);
        void Set<T>(string key, T value, TimeSpan duration, string prefix);
        void RemoveByPrefix(string prefix);
    }
}