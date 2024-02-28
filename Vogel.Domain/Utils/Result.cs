namespace Vogel.Domain.Utils
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
        public Result<T> Success(T value) => new Result<T>(value);
        public Result<T> Failure(Exception exception) => new Result<T>(exception);
    }
}
