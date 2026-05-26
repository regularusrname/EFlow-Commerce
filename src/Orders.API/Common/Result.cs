namespace Orders.API.Common;

public class Result
{
    protected Result(bool isSuccess, Error? error = default)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; init; }

    public static Result Success() => new(isSuccess: true, null);
    public static Result Failure(Error error) => new(isSuccess: false, error);
}

public class Result<T> : Result
{
    protected Result(bool isSuccess, Error? error) 
        : base(isSuccess, error) {}

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
}
