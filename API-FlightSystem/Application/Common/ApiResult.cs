namespace Application.Common
{
    public class ApiResult<T>
    {
        public bool Succeeded { get; set; }
        public T? Result { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();

        public ApiResult() { }

        public ApiResult(bool succeeded, T result, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Result = result;
            Errors = errors;
        }

        public static ApiResult<T> Success(T result) => new(true, result, Array.Empty<string>());

        public static ApiResult<T?> Failure(IEnumerable<string> errors) => new(false, default, errors);
    }
}
