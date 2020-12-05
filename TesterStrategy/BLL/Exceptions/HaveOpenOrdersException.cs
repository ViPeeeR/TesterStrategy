using System;

namespace TesterStrategy.BLL.Exceptions
{
    public class HaveOpenOrdersException : Exception
    {
        public HaveOpenOrdersException(string message)
            : base(message)
        {
        }
    }
}