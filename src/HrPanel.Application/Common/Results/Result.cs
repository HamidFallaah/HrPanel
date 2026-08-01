namespace HrPanel.Application.Common.Results;

public class Result : IResult<Result>
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new ArgumentException("یک نتیجه موفق نمی‌تواند شامل خطا باشد", nameof(error));
        }

        if (!isSuccess && error == Error.None)
        {
            throw new ArgumentException("یک نتیجه ناموفق باید حاوی یک خطا باشد", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public static Result Success()
    {
        return new Result(true, Error.None);
    }
    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
}