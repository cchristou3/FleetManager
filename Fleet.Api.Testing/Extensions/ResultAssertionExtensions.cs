using Fleet.Api.Shared;
using FluentAssertions;

namespace Fleet.Api.Testing.Extensions;

public static class ResultAssertionExtensions
{
    public static void ShouldBeThisFailure<TValue>(this Result<TValue> actual, (ResultCode Code, Error Error) expected)
    {
        actual.IsSuccessful.Should().BeFalse();
        actual.Error.Message.Should().Be(expected.Error.Message);
        actual.Code.Should().Be(expected.Code);
    }

    public static void ShouldBeSuccess<TValue>(this Result<TValue> actual)
    {
        actual.IsSuccessful.Should().BeTrue();
        actual.Code.Should().Be(ResultCode.Success);
    }
}