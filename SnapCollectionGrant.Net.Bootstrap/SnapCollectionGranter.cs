using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnapCollectionGrant.Net.Bootstrap
{
    internal static class SnapCollectionGranter
    {
        [Flags]
        public enum Access : uint
        {
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            PROCESS_ALL_ACCESS = 0x1F0FFF
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        private static void DisplayMessageWarning(string m)
        {
           MessageBox.Show(m, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void DisplayMessageOk(string m)
        {
            MessageBox.Show(m, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Grant()
        {
            var assemblies = IL2CppApi.IL2CppDumper.GetLoadedAssemblies();
            var appState = assemblies.Where(a => a.Name.Contains("App.State")).FirstOrDefault();

            if (appState == null)
            {
                DisplayMessageWarning("IL2Cpp could not locate assembly, failed!");
                return;
            }

            var collectionManager = appState.GetClasses().Where(c => c.Name.Contains("CollectionManager")).FirstOrDefault();

            if (collectionManager == null)
            {
                DisplayMessageWarning("IL2Cpp could not locate class, failed!");
                return;
            }

            var methodAddNewDeck = collectionManager.GetMethods().Where(m => m.Name.Contains("AddNewDeck")).FirstOrDefault();

            if (methodAddNewDeck == null)
            {
                DisplayMessageWarning("IL2Cpp could not locate method1, failed!");
                return;
            }

            var methodGrantAll = collectionManager.GetMethods().Where(m => m.Name.Contains("DebugGrantAllCards")).FirstOrDefault();

            if (methodGrantAll == null)
            {
                DisplayMessageWarning("IL2Cpp could not locate method2, failed!");
                return;
            }

            // set up redirect
            //
            var ptr = methodAddNewDeck.MethodPointer;
            var processHandle = OpenProcess((uint) (Access.PROCESS_VM_OPERATION | Access.PROCESS_VM_READ | Access.PROCESS_VM_WRITE), false, Process.GetCurrentProcess().Id);

            if (!VirtualProtectEx(processHandle, ptr, new IntPtr(4096), 0x40, out _))
            {
                DisplayMessageWarning($"VirtualProtect failed with code {GetLastError()}");
                return;
            }

            unsafe
            {
                var uPtr = (byte*)ptr;

                *(uPtr) = 0x48;
                *(uPtr+1) = 0xB8;
                *((long*)(uPtr + 2)) = methodGrantAll.MethodPointer.ToInt64();


                *(uPtr + 10) = 0x48;
                *(uPtr + 11) = 0x8B;
                *(uPtr + 12) = 0xD0;

                *(uPtr + 13) = 0xFF;
                *(uPtr + 14) = 0xE0;
            }

            DisplayMessageOk("Injected, click create new deck button to load all cards. Make sure to close the game (exit the proccess) when you are done.");
        }
    }
}
