namespace BuildingBlocks.Common.Results;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None) throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == Error.None) throw new InvalidOperationException("Failure result must have an error.");
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failure result.");

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");
    public static readonly Error NotFound = new("Error.NotFound", "Resource was not found.");
    public static readonly Error Unauthorized = new("Error.Unauthorized", "Unauthorized access.");
    public static readonly Error Forbidden = new("Error.Forbidden", "Access forbidden.");
    public static readonly Error Conflict = new("Error.Conflict", "A conflict occurred.");
    public static readonly Error Validation = new("Error.Validation", "A validation error occurred.");

    public static Error Custom(string code, string message) => new(code, message);
}
