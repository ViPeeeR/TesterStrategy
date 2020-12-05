using System;

namespace TesterStrategy.BLL.Exceptions
{
    public class MarginFreeNotEnoughException : Exception
    {
        public MarginFreeNotEnoughException(string message)
            : base(message)
        {
        }
    }
}