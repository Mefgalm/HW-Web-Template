using System;

namespace __Project__.Common.Invoke
{
    public interface IInvokeResultSettings : ICloneable
    {
        OperationLogSettings ValidationLogSettings  { get; }
        OperationLogSettings LogRequestLogSettings  { get; }
        InvokeFunctionLogSetttings InvokeFunctionLogSetttings { get; }
    }
}