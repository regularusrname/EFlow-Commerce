namespace Orders.API.Common;

public class Result
{
    protected Result(bool isSuccess, Error? error = default)
    {
        IsSuccess = isSuccess;
        Error = error is null ? null : [error];
    }

    protected Result(bool isSuccess, IReadOnlyCollection<Error> errors)
    {
        IsSuccess = isSuccess;
        Error = errors.Count == 0 || errors is null ? null : [..errors];
    }

    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyCollection<Error>? Error { get; init; }

    public static Result Success() => new(isSuccess: true, error: null);
    public static Result Failure(Error error) => new(isSuccess: false, error);
    public static Result Failure(IReadOnlyCollection<Error> errors) => new(isSuccess: false, [..errors]);
}

public class Result<T> : Result
{
    protected Result(bool isSuccess, Error? error) 
        : base(isSuccess, error) {}
    
    protected Result(bool isSuccess, IReadOnlyCollection<Error> errors) 
        : base(isSuccess, [..errors]) {}


    public T? Value
    {
        get => IsSuccess
            ? field
            : throw new InvalidOperationException("Cannot access value of a failed result");
        init;
    }

    public static Result<T> Success(T value) 
        => new(isSuccess: true, error: null)
        {
            Value = value
        };

    public new static Result<T> Failure(Error error) 
        => new(isSuccess: false, error: error);

    public new static Result<T> Failure(IReadOnlyCollection<Error> errors)
        => new(isSuccess: false, errors: [..errors]);
}
