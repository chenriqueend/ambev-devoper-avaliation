using System;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class QueryResult<T>
    {
        public bool Success { get; }
        public T? Data { get; }
        public string? Error { get; }

        protected QueryResult(bool success, T? data, string? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }

        public static QueryResult<T> CreateSuccess(T data)
        {
            return new QueryResult<T>(true, data, null);
        }

        public static QueryResult<T> CreateFailure(string error)
        {
            return new QueryResult<T>(false, default, error);
        }
    }
}