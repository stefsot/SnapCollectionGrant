using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SnapCollectionGrant.Net.Bootstrap
{
    public class Bootstrap
    {
        static Bootstrap()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public static void Init(string settings)
        {
            SnapCollectionGranter.Grant();
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name).Name;
            var directory = Path.GetDirectoryName(args.RequestingAssembly.Location);

            Debug.WriteLine($"resolving \"{assemblyName}\"");

            if (directory == null)
                return null;

            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                if (string.Equals(assemblyName, Path.GetFileNameWithoutExtension(file), StringComparison.InvariantCultureIgnoreCase))
                    return Assembly.LoadFile(file);
            }

            Debug.WriteLine($"resolve of \"{assemblyName}\" failed!");

            return null;
        }
    }
}
