namespace Catalog.API.Common;

public class Result
{
    public Result(bool isSuccess, Error? error = null)
    {
        IsSuccess = isSuccess;
        Errors = error is null ? [] : [error];
    }

    public Result(bool isSuccess, IReadOnlyCollection<Error>? errors = null)
    {
       IsSuccess = isSuccess;
       Errors = errors is null ? [] : [..errors];
    }

    public bool IsSuccess { get; set; }
    public IReadOnlyCollection<Error>? Errors { get; init; }

    public static Result Failure(IReadOnlyCollection<Error> errors) => new(isSuccess: false, errors);
    public static Result Failure(Error error) => new(isSuccess: false, error);
    public static Result Success() => new(isSuccess: true, error: null);
}

public class Result<T> : Result
{
    public Result(bool isSuccess, Error? error = null) : base(isSuccess, error) {}

    public Result(bool isSuccess, IReadOnlyCollection<Error>? errors = null) : base(isSuccess, errors) {}

    public T? Value 
    { 
        get => IsSuccess 
            ? field 
            : throw new InvalidOperationException("Cannot get Value property: Result is not failure"); 
        private set; 
    }

    public static Result<T> Success(T value) => new(true, error: null)
    {
        Value = value
    };

    public new static Result<T> Failure(IReadOnlyCollection<Error> errors) => new (false, errors: errors);
    public new static Result<T> Failure(Error error) => new(false, error: error);
}
