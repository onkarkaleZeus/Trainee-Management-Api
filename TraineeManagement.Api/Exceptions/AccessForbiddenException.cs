using System;

namespace TraineeManagement.Api.Exceptions
{

    public class AccessForbiddenException : Exception
    {
        public AccessForbiddenException(string message) : base(message) { }
    }
}