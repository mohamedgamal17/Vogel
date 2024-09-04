using Vogel.BuildingBlocks.Shared.Results;

namespace Vogel.BuildingBlocks.Shared.Extensions
{
    public static class ResultExtensions
    {
        public static void ThrowIfFailure<T>(this Result<T> result)
        {
            if (result.IsFailure)
            {
                throw result.Exception!;
            }          
        }
    }
}
