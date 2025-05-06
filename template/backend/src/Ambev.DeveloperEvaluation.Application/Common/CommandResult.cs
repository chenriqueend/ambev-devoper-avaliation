using System;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class CommandResult<T>
    {
        public bool Success { get; }
        public T? Data { get; }
        public string? Error { get; }

        protected CommandResult(bool success, T? data, string? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }

        public static CommandResult<T> CreateSuccess(T data)
        {
            return new CommandResult<T>(true, data, null);
        }

        public static CommandResult<T> CreateFailure(string error)
        {
            return new CommandResult<T>(false, default, error);
        }
    }
}