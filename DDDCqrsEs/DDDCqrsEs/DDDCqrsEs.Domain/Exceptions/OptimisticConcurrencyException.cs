using System;

namespace DDDCqrsEs.Domain.Exceptions
{
    public class OptimisticConcurrencyException : Exception
    {
        public const string DefaultMessage = "Data was changed by another user.";

        public OptimisticConcurrencyException(Exception innerException = null)
            : base(DefaultMessage, innerException)
        {
        }
    }
}
