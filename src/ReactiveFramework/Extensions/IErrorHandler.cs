namespace ReactiveFramework.Extensions;

public interface IErrorHandler
{
    void HandleException(Exception exception);
}