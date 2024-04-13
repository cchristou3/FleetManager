using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Fleet.Api.Shared;

public class Result<TValue>
{
    protected Result(TValue value, ResultCode code, Error error)
    {
        Value = value;
        Code = code;
        Error = error;
    }

    public ResultCode Code { get; }

    public TValue Value { get; }

    public Error Error { get; }

    public bool IsSuccessful => Code == ResultCode.Success && Error is null;

    public bool HasFailed => !IsSuccessful;

    public static Result<TValue> Failure(ResultCode code, Error error)
    {
        return new Result<TValue>(default, code, error);
    }

    public static Result<TValue> Failure((ResultCode Code, Error Error) data)
    {
        return new Result<TValue>(default, data.Code, data.Error);
    }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value, ResultCode.Success, default);
    }

    public static Result<TValue> Success()
    {
        return new Result<TValue>(default, ResultCode.Success, default);
    }

    public Result<TDestination> ToResult<TDestination>()
    {
        return new Result<TDestination>(default, Code, Error);
    }

    public IActionResult ToActionResult(ControllerBase invoker)
    {
        return Code switch
        {
            ResultCode.Success => Value is not null ? invoker.Ok(Value) : invoker.NoContent(),
            ResultCode.ValidationError => invoker.BadRequest(Error),
            ResultCode.NotFound => invoker.NotFound(Error),
            ResultCode.ServerError => invoker.Problem(
                title: Error.Message,
                statusCode: (int)HttpStatusCode.InternalServerError),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}