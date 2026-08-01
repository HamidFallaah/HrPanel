namespace HrPanel.Application.Common.Results;

public sealed class Result<TValue>: Result, IResult<Result<TValue>>
{
    private readonly TValue? _value;
    private Result(TValue? value,bool isSuccess,Error error): base(isSuccess, error)
    {
        _value = value;
    }
    public TValue Value => IsSuccess? _value!: throw new InvalidOperationException("به مقدار یک نتیجه ناموفق نمی ‌توان دسترسی پیدا کرد");
    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value,true,Error.None);
    }
    public new static Result<TValue> Failure(Error error)
    {
        return new Result<TValue>(default,false,error);
    }
}