using CoreLib.TraceLogic.Interfaces;
using Serilog.Context; 
using System;
using Microsoft.AspNetCore.Http;

namespace CoreLib.TraceLogic
{
    public interface ITraceIdAccessor
    {
        string? CurrentTraceId { get; }
    }

    internal class TraceIdAccessor : ITraceReader, ITraceWriter, ITraceIdAccessor
    {
        public string Name => "X-Trace-Id";

        private string? _value; 

        public string GetValue()
        {
            return _value ?? string.Empty;
        }

        public void WriteValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = Guid.NewGuid().ToString();
            }

            _value = value;
           
            LogContext.PushProperty(Name, value); 
        }

        public string? CurrentTraceId => _value;
    }
}