namespace Application.Common;

public readonly struct Result<TValue, TError>
{
    public readonly TValue? _value;
    public readonly TError? _error;
    public bool IsError { get; }
    public bool IsSuccess => !IsError;

    private Result(TValue value)
    {
        IsError = false;
        _value = value;
        _error = default;
    }

    private Result(TError error)
    {
        IsError = true;
        _value = default;
        _error = error;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
        => IsError ? onError(_error!) : onSuccess(_value!);
}
