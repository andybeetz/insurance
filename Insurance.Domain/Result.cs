using System.Text.Json.Serialization;

namespace Insurance.Domain;

// This probably belongs in Insurance.Core but right now it seems pointless to create a library to hold 
// this one set of objects

public enum ErrorType
{
    Failure = 0,
    NotFound = 1,
    Validation = 2,
    Conflict = 3
}

public class Error
{
    private Error(
        string code,
        string description,
        ErrorType errorType
    )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType ErrorType { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
}

public class Result
{
    protected Result()
    {
        IsSuccess = true;
        Error = default;
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; }

    public static implicit operator Result(Error error) =>
        new(error);

    public static Result Success() =>
        new();

    public static Result Failure(Error error) =>
        new(error);
}

public sealed class Resulting<TValue> : Result
{
    private Resulting(
        TValue value
    ) : base()
    {
        Value = value;
    }

    private Resulting(
        Error error
    ) : base(error)
    {
        Value = default;
    }

    public TValue Value =>
        IsSuccess ? field! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

    public static implicit operator Resulting<TValue>(Error error) =>
        new(error);

    public static implicit operator Resulting<TValue>(TValue value) =>
        new(value);

    public static Resulting<TValue> Success(TValue value) =>
        new(value);

    public new static Resulting<TValue> Failure(Error error) =>
        new(error);
}