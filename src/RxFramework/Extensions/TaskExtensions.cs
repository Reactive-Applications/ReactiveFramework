namespace RxFramework.Extensions;

public static class TaskExtensions
{
    public static async void Await(this Task task, IErrorHandler? errorHandler = null)

    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            errorHandler?.HandleException(ex);
        }
    }

    public static async void Await(this Task task, Action completedCallBack, IErrorHandler? errorHandler = null)
    {
        try
        {
            await task;
            completedCallBack?.Invoke();
        }
        catch (Exception ex)
        {
            errorHandler?.HandleException(ex);
        }
    }

    public static async void Await<T>(this Task<T> task, Action<T> completedCallBack, IErrorHandler? errorHandler = null)
    {
        try
        {
            var result = await task;
            completedCallBack?.Invoke(result);
        }
        catch (Exception ex)
        {
            errorHandler?.HandleException(ex);
        }
    }
}
