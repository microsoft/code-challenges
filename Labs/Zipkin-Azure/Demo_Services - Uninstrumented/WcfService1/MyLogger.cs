using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Criteo.Profiling.Tracing;

namespace WcfService1
{
    class MyLogger : ILogger
    {
        public MyLogger()
        {

        }
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }

        public void LogInformation(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            Console.WriteLine(message);
        }

    }
}
