namespace Vogel.BuildingBlocks.Shared.Results
{
    public class Result<T>
    {
        public T? Value { get; private set; }
        public Exception? Exception { get; private set; }
        public bool IsSuccess => _success;
        public bool IsFailure => !_success;

        private readonly bool _success = false;
        public Result(T value)
        {
            Value = value;
            _success = true;
        }

        public Result(Exception exception)
        {
            _success = false;
            Exception = exception;
        }

        public static implicit operator Result<T>(T value) => new Result<T>(value);
    }
}
