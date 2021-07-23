using System;
using System.Runtime.Serialization;

namespace ProcessMonitoring
{
    public class WrongAmmountOfArgumentsException : Exception
    {
        public WrongAmmountOfArgumentsException() {}

        public WrongAmmountOfArgumentsException(string message) : base(message) {}
    }
}