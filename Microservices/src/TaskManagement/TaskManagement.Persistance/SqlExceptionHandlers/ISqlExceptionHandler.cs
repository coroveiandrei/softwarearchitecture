using System;

namespace TaskManagement.Persistance.SqlExceptionHandlers
{
    public interface ISqlExceptionHandler
    {
        void Handle(Exception exception);
    }
}
