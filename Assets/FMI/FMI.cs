using System;
using System.Runtime.InteropServices;

namespace FMI
{
    public abstract class FMU
    {
#if UNITY_STANDALONE_WIN
        [DllImport("kernel32", SetLastError = true)]
        protected static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        protected static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static extern bool FreeLibrary(IntPtr hModule);
#else
        protected const int RTLD_NOW = 2;

        [DllImport("libdl")]
		protected static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libdl")]
		protected static extern int dlclose(IntPtr handle);

		[DllImport("libdl")]
		private static extern IntPtr dlsym(IntPtr handle, String symbol);
#endif

        protected IntPtr dll;

        protected TDelegate getFunc<TDelegate>(string fname) where TDelegate : class
        {
#if UNITY_STANDALONE_WIN
            IntPtr p = GetProcAddress(dll, fname);
#else
            IntPtr p = dlsym(dll, fname);
#endif

            if (p == IntPtr.Zero)
            {
                throw new Exception("Could not get function address for " + fname);
            }

            Delegate function = Marshal.GetDelegateForFunctionPointer(p, typeof(TDelegate));

            // Ideally, we'd just make the constraint on TDelegate be
            // System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
            // prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
            object o = function;

            return (TDelegate)o;
        }
        
        public bool LogFMICalls = false;
    }
}