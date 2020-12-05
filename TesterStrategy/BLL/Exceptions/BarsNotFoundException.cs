using System;

namespace TesterStrategy.BLL.Exceptions
{
    public class BarsNotFoundException : Exception
    {
        public BarsNotFoundException(string message)
            : base(message)
        {
        }
    }
}