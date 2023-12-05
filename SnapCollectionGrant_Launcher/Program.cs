using ManagedInjector;
using SnapCollectionGrant.Net.Bootstrap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SnapCollectionGrant_Launcher
{
    public static class SnapInjector
    {
        public static Process GetActiveSnapProcess()
        {
            return Process.GetProcessesByName("SNAP").FirstOrDefault();
        }

        public static bool Inject(string settings = null)
        {
            var process = GetActiveSnapProcess();

            if (process == null)
                return false;

            var type = typeof(Bootstrap);

            var transportData = new InjectorData
            {
                AssemblyName = type.Assembly.Location,
                ClassName = type.FullName,
                MethodName = "Init",
                SettingsFile = settings
            };

            Injector.Launch(process.MainWindowHandle, transportData);

            return true;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"injection result: {(SnapInjector.Inject() ? "success" : "failed")}");
            Console.ReadKey(true);
        }
    }
}
