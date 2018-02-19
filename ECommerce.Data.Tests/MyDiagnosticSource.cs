using System;
using System.Diagnostics;

namespace ECommerce.Data.Tests
{
    internal class MyDiagnosticSource : DiagnosticSource
    {
        public override bool IsEnabled(string name)
        {
            return true;
        }

        public override void Write(string name, object value)
        {
            Console.Write(name);
        }
    }
}