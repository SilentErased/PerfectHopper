using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Memory
{
    public class Process
    {
        private IntPtr handle;
        private System.Diagnostics.Process proc;

        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtWriteVirtualMemory(IntPtr ProcessHandle, IntPtr BaseAddress, byte[] Buffer, uint NumberOfBytesToWrite, out uint NumberOfBytesWritten);

        public Process(string processName)
        {
            var procs = System.Diagnostics.Process.GetProcessesByName(processName);
            if (procs.Length == 0)
                throw new ArgumentException("Process not found");
            proc = procs[0];
            handle = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, proc.Id);
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Cannot open process");
        }

        public T ReadAddress<T>(long address) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];
            if (!ReadProcessMemory(handle, new IntPtr(address), buffer, size, out IntPtr bytesRead) || bytesRead.ToInt32() != size)
                throw new InvalidOperationException("Read failed");
            var handleGC = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handleGC.AddrOfPinnedObject());
            }
            finally
            {
                handleGC.Free();
            }
        }
        public byte[] ReadMemory(long address, int size)
        {
            byte[] buffer = new byte[size];
            if (!ReadProcessMemory(handle, new IntPtr(address), buffer, size, out IntPtr bytesRead)
                || bytesRead.ToInt32() != size)
                throw new InvalidOperationException("ReadMemory failed");
            return buffer;
        }
        public IntPtr GetBaseAddress()
        {
            return proc.MainModule.BaseAddress;
        }

        public Dictionary<string, IntPtr> GetModules()
        {
            var modules = new Dictionary<string, IntPtr>();
            foreach (ProcessModule m in proc.Modules)
                modules[m.ModuleName] = m.BaseAddress;
            return modules;
        }

        public int WriteNtMemory(IntPtr address, byte[] buffer)
        {
            return NtWriteVirtualMemory(handle, address, buffer, (uint)buffer.Length, out _);
        }
        public string ReadString(long address, int maxLength)
        {
            byte[] buffer = new byte[maxLength];
            if (!ReadProcessMemory(handle, new IntPtr(address), buffer, maxLength, out IntPtr bytesRead) || bytesRead.ToInt32() == 0)
                throw new InvalidOperationException("Read failed or no data");
            int actualLength = Array.IndexOf(buffer, (byte)0);
            if (actualLength < 0) actualLength = bytesRead.ToInt32();
            return Encoding.UTF8.GetString(buffer, 0, actualLength);
        }
        private string decompress(byte[] data) { throw new NotImplementedException(); }
    }
}
