﻿using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
namespace Vogel.Application.Tests.Utilites
{
    public class TestOutputLoggerFactory : ILoggerFactory
    {
        readonly bool _enabled;

        public TestOutputLoggerFactory(bool enabled)
        {
            _enabled = enabled;
        }

        public TestExecutionContext Current { get; set; }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
        {
            return new TestOutputLogger(this, _enabled);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
