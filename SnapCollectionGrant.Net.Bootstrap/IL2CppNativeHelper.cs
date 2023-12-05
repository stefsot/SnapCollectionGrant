using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SnapCollectionGrant.Net.Bootstrap
{
    internal static class IL2CppNativeHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        public delegate IntPtr _delegate_il2cpp_domain_get();



        static IL2CppNativeHelper()
        {
        }

        public static T GetNativeDelegate<T>(IntPtr ptr) where T : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        }

        public static T GetIL2CppFunc<T>(string name) where T : Delegate
        {
            var module = GetModuleHandle("GameAssembly.dll");
            var ptr = GetProcAddress(module, name);

            return GetNativeDelegate<T>(ptr);
        }
    }
}
