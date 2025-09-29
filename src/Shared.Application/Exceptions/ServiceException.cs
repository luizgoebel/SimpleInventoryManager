namespace Shared.Application.Exceptions;

public class ServiceException : Exception
{
    public object ObjetoErro { get; set; }

    public ServiceException(string message)
        : base(message) { }

    public ServiceException(string message, object obj)
        : base(message)
    {
        ObjetoErro = obj;
    }
}
