namespace Orders.API.Common;

public interface IResult<TSelf> where TSelf : IResult<TSelf>
{
    bool IsSuccess { get; }
    IReadOnlyCollection<Error> Errors { get; }
    static abstract TSelf Failure(IReadOnlyCollection<Error> errors);
    static abstract TSelf Failure(Error error);
}

public class Result<T> : IResult<Result<T>>
{
    public T? Value
    {
        get => IsSuccess
            ? field
            : throw new InvalidOperationException("Cannot access value of a failed result");
        init;
    }
    public bool IsSuccess { get; }
    public IReadOnlyCollection<Error> Errors { get; }
    
    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Errors = [];
    }

    private Result(IReadOnlyCollection<Error> errors)
    {
        if (errors is null || !errors.Any())
            throw new InvalidOperationException("Failure result must contain at least one error.");
        IsSuccess = false;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(IReadOnlyCollection<Error> errors) => new(errors);
    public static Result<T> Failure(Error error) => new([error]);
}
