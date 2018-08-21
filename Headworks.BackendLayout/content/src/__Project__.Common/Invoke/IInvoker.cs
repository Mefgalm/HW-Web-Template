using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace __Project__.Common.Invoke
{
    public class BaseInvokeHandler<K> : IInvokeHandler<K>
    {
        private readonly IInvokeResultSettings _invokeResultSettings;
        private readonly ILogger<K>            _logger;

        public BaseInvokeHandler(IInvokeResultSettings invokeResultSettings, ILogger<K> logger)
        {
            _invokeResultSettings = invokeResultSettings;
            _logger               = logger;
        }

        public InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func, object request)
        {
            return Invoke(func, request, _ => _invokeResultSettings);
        }

        public InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func, object request,
                                         Func<IInvokeResultSettings, IInvokeResultSettings> settingsFunc)
        {
            var settings = settingsFunc((IInvokeResultSettings) _invokeResultSettings.Clone());

            LogRequest(request, settings.LogRequestLogSettings);

            var validationRules = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, new ValidationContext(request), validationRules, true))
            {
                LogValidation(validationRules, settings.ValidationLogSettings);
                return InvokeResult<T>.Fail(ResultCode.ValidationError,
                    string.Join(" ",
                        validationRules.Select(x => $"\"{x.MemberNames.FirstOrDefault()}\":\"{x.ErrorMessage}\"")));
            }

            var result = func();
            LogInvoke(result, settings.InvokeFunctionLogSetttings);

            return result;
        }

        public IInvokeResultSettings InvokeResultSettings => _invokeResultSettings;

        public void SetInvokeResultSettings(Action<IInvokeResultSettings> settingsAction)
        {
            settingsAction(_invokeResultSettings);
        }

        public InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func)
        {
            return Invoke(func, _ => _invokeResultSettings);
        }

        public InvokeResult<T> Invoke<T>(Func<InvokeResult<T>> func,
                                         Func<IInvokeResultSettings, IInvokeResultSettings> settingsFunc)
        {
            var settings = settingsFunc((IInvokeResultSettings) _invokeResultSettings.Clone());
            var result   = func();
            LogInvoke(result, settings.InvokeFunctionLogSetttings);

            return result;
        }

        protected virtual void LogRequest(object request, OperationLogSettings operationLogSettings)
        {
            if (operationLogSettings.IsLogging)
            {
                _logger.Log(operationLogSettings.LogLevel, "{@request}", request);
            }
        }

        protected virtual void LogValidation(IEnumerable<ValidationResult> validationResults,
                                             OperationLogSettings operationLogSettings)
        {
            if (operationLogSettings.IsLogging)
            {
                _logger.Log(operationLogSettings.LogLevel, "Validation failed");
            }
        }

        protected virtual void LogInvoke<T>(InvokeResult<T> result, InvokeFunctionLogSetttings operationLogSettings)
        {
            switch (operationLogSettings.LogInvokeResult)
            {
                case LogInvokeResult.Always:
                    _logger.Log(operationLogSettings.LogLevel, "{@result}", result);
                    break;
                case LogInvokeResult.IsFail:
                    if (!result.IsSuccess)
                        _logger.Log(operationLogSettings.LogLevel, "{@result}", result);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}