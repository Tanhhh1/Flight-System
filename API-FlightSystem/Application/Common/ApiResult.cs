namespace Application.Common
{
    public record FieldError(string? PropertyName, string ErrorMessage);
    public class ApiResult<T>
    {
        public bool Succeeded { get; set; }
        public T? Result { get; set; }
        public IEnumerable<FieldError> Errors { get; set; } = Array.Empty<FieldError>();

        public ApiResult() { }

        public ApiResult(bool succeeded, T? result, IEnumerable<FieldError> errors)
        {
            Succeeded = succeeded;
            Result = result;
            Errors = errors;
        }

        public static ApiResult<T> Success(T result) => new(true, result, Array.Empty<FieldError>());

        public static ApiResult<T> Failure(string message) => new(false, default, [new FieldError(null, message)]);

        public static ApiResult<T> Failure(IEnumerable<FieldError> errors) => new(false, default, errors);
    }
}
