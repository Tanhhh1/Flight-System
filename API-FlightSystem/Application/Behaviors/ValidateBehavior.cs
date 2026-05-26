using Application.Common;
using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
    public class ValidateBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidateBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var fieldErrors = (await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .Select(f => new FieldError(f.PropertyName, f.ErrorMessage))
                .ToArray();

            if (!fieldErrors.Any())
                return await next();

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ApiResult<>))
            {
                var innerType = typeof(TResponse).GenericTypeArguments[0];
                var failureMethod = typeof(ApiResult<>)
                    .MakeGenericType(innerType)
                    .GetMethod(nameof(ApiResult<object>.Failure), [typeof(IEnumerable<FieldError>)]);

                return (TResponse)failureMethod!.Invoke(null, new object[] { fieldErrors })!;
            }

            throw new ValidationException(fieldErrors.Select(e => new FluentValidation.Results.ValidationFailure(e.PropertyName, e.ErrorMessage)));
        }
    }
}