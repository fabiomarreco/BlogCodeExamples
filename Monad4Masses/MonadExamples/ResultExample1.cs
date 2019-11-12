using System;

namespace Monad4Examples.ResultExample1
{
    public class Result<TSuccess, TFailure>
    {
        public Result(TSuccess success)
        {
            Success = success;
            IsSuccess = true;
        }

        public Result(TFailure failure)
        {
            Failure = failure;
            IsSuccess = false;
        }

        public bool IsSuccess { get; }
        public TSuccess Success { get; }
        public TFailure Failure { get; }
    }

    


}