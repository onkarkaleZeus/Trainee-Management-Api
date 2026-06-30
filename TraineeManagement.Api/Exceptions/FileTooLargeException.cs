using System;

namespace TraineeManagement.Api.Exceptions
{

    public class FileTooLargeException : Exception
    {
        public FileTooLargeException(string message) : base(message) { }
    }
}