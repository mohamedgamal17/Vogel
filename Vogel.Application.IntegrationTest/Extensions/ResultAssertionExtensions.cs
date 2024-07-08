using FluentAssertions;
using Vogel.BuildingBlocks.Domain.Results;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class ResultAssertionExtensions
    {
        public static void ShouldBeSuccess<T>(this Result<T> result)
        {
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Value.Should().NotBeNull();
        }

        public static void ShoulBeFailure<T>(this Result<T> result , Type exceptionType)
        {
            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.Exception.Should().BeOfType(exceptionType);
        }
    }
}
