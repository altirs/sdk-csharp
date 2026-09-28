using System;

namespace Altirs.Sdk
{
    public class AltirsException : Exception
    {
        public int Status { get; }
        public string RequestId { get; }

        public AltirsException(string message, int status, string requestId)
            : base(message)
        {
            Status = status;
            RequestId = requestId;
        }
    }
}
