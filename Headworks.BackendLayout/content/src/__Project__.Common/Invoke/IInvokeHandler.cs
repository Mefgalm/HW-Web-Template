using System;

namespace __Project__.Common.Invoke
{
    public interface IInvokeHandler<K>
    {
        IInvokeResultSettings InvokeResultSettings { get; }
        void SetInvokeResultSettings(Action<IInvokeResultSettings> settingsAction);
        
        InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func);
        InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func, Func<IInvokeResultSettings, IInvokeResultSettings> settingsFunc);
        
        InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func, object request);
        InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func, object request, Func<IInvokeResultSettings, IInvokeResultSettings> settingsFunc);
    }
}