﻿namespace RxFramework.Extensions;

public interface IErrorHandler
{
    void HandleException(Exception exception);
}