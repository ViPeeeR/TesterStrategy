using System;

namespace TesterStrategy.BLL.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(string message)
            : base(message)
        {
        }
    }
}