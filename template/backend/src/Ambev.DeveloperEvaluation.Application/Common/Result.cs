using System;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; }
        public T? Data { get; }
        public string? Error { get; }

        protected Result(bool success, T? data, string? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }

        public static Result<T> CreateSuccess(T data)
        {
            return new Result<T>(true, data, null);
        }

        public static Result<T> CreateFailure(string error)
        {
            return new Result<T>(false, default, error);
        }
    }
}