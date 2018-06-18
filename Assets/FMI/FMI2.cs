using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;


namespace FMI2 {

	using size_t = System.UInt64;
	using fmi2String = System.String;
	using fmi2ComponentEnvironment = System.IntPtr;
	using fmi2Status = System.Int32;
    using fmi2Real = System.Double;
    using fmi2Integer = System.Int32;
	using fmi2Boolean = System.Int32;
	using fmi2Component = System.IntPtr;
	using fmi2ValueReference = System.UInt32;

	public enum fmi2Type
	{
		fmi2ModelExchange,
		fmi2CoSimulation
	};

	//typedef void      (*fmi2CallbackLogger)        (fmi2ComponentEnvironment, fmi2String, fmi2Status, fmi2String, fmi2String, ...);
	public delegate void fmi2CallbackLogger(IntPtr componentEnvironment, IntPtr instanceName, fmi2Status status, IntPtr category, IntPtr message); //, IntPtr args);

	//typedef void*     (*fmi2CallbackAllocateMemory)(size_t, size_t);
	public delegate IntPtr fmi2CallbackAllocateMemory(size_t a, size_t b);

	//typedef void      (*fmi2CallbackFreeMemory)    (void*);
	public delegate void fmi2CallbackFreeMemory(IntPtr mem);

	//typedef void      (*fmi2StepFinished)          (fmi2ComponentEnvironment, fmi2Status);
	public delegate void fmi2StepFinished(fmi2ComponentEnvironment mem, fmi2Status status);

	/* Creation and destruction of FMU instances and setting debug status */
	//  fmi2Component fmi2Instantiate(fmi2String  instanceName,
	//      fmi2Type    fmuType,
	//      fmi2String fmuGUID,
	//      fmi2String fmuResourceLocation,
	//      const fmi2CallbackFunctions* functions, fmi2Boolean visible, fmi2Boolean loggingOn);
	delegate IntPtr fmi2InstantiateDelegate(
		string instanceName,
		int fmuType,
		string fmuGUID,
		string fmuResourceLocation,
		IntPtr callbacks,
        fmi2Boolean visible,
        fmi2Boolean loggingOn);

	// typedef void          fmi2FreeInstanceTYPE(fmi2Component);
	delegate void fmi2FreeInstanceDelegate(fmi2Component c);


	//  fmi2Status fmi2SetupExperiment(fmi2Component c,
	//      fmi2Boolean   toleranceDefined,
	//      fmi2Real      tolerance,
	//      fmi2Real      startTime,
	//      fmi2Boolean   stopTimeDefined,
	//      fmi2Real      stopTime);
	delegate fmi2Status fmi2SetupExperimentDelegate(
		fmi2Component c,
		fmi2Boolean toleranceDefined,
		fmi2Real tolerance,
		fmi2Real startTime,
		fmi2Boolean stopTimeDefined,
		fmi2Real stopTime);

	// fmi2Status fmi2EnterInitializationMode(fmi2Component c);
	delegate fmi2Status fmi2EnterInitializationModeDelegate(fmi2Component c);

	// fmi2Status fmi2ExitInitializationMode(fmi2Component c);
	delegate fmi2Status fmi2ExitInitializationModeDelegate(fmi2Component c);

	// typedef fmi2Status fmi2TerminateTYPE(fmi2Component);
	delegate fmi2Status fmi2TerminateDelegate(fmi2Component c);

	// typedef fmi2Status fmi2ResetTYPE(fmi2Component);
	delegate fmi2Status fmi2ResetDelegate(fmi2Component c);

	//  fmi2Status fmi2DoStep(fmi2Component c,
	//      fmi2Real    currentCommunicationPoint,
	//      fmi2Real    communicationStepSize,
	//      fmi2Boolean noSetFMUStatePriorToCurrentPoint);
	delegate fmi2Status fmi2DoStepDelegate(
		fmi2Component c,
		fmi2Real currentCommunicationPoint,
		fmi2Real communicationStepSize,
		fmi2Boolean noSetFMUStatePriorToCurrentPoint);

	// fmi2Status fmi2GetReal (fmi2Component c, const fmi2ValueReference vr[], size_t nvr, fmi2Real value[]);
	delegate fmi2Status fmi2GetRealDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Real[] value);

	// typedef fmi2Status fmi2SetRealTYPE   (fmi2Component, const fmi2ValueReference[], size_t, const fmi2Real   []);
	delegate fmi2Status fmi2SetRealDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Real[] value);

    delegate fmi2Status fmi2GetIntegerDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Integer[] value);

    delegate fmi2Status fmi2SetIntegerDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Integer[] value);

    delegate fmi2Status fmi2GetBooleanDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Boolean[] value);

    delegate fmi2Status fmi2SetBooleanDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Boolean[] value);

    delegate fmi2Status fmi2GetStringDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, IntPtr[] value);

    delegate fmi2Status fmi2SetStringDelegate(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] fmi2String[] value);

	//  typedef struct {
	//      const fmi2CallbackLogger         logger;
	//      const fmi2CallbackAllocateMemory allocateMemory;
	//      const fmi2CallbackFreeMemory     freeMemory;
	//      const fmi2StepFinished           stepFinished;
	//      const fmi2ComponentEnvironment   componentEnvironment;
	//  } fmi2CallbackFunctions;
	public struct fmi2CallbackFunctions
	{
		public fmi2CallbackLogger         logger;
		//public IntPtr logger;
		public fmi2CallbackAllocateMemory allocateMemory;
		public fmi2CallbackFreeMemory freeMemory;
		public fmi2StepFinished stepFinished;
		public fmi2ComponentEnvironment componentEnvironment;
	}

	public class FMU : IDisposable
	{
        
#if UNITY_STANDALONE_OSX
        const int RTLD_NOW = 2;
#endif

        const int fmi2True = 1;
		const int fmi2False = 0;

		IntPtr dll;
        fmi2Component component;
		IntPtr callbacks;

		fmi2InstantiateDelegate fmi2Instantiate;
		fmi2FreeInstanceDelegate fmi2FreeInstance;
		fmi2SetupExperimentDelegate fmi2SetupExperiment;
		fmi2EnterInitializationModeDelegate fmi2EnterInitializationMode;
		fmi2ExitInitializationModeDelegate fmi2ExitInitializationMode;
		fmi2TerminateDelegate fmi2Terminate;
		fmi2ResetDelegate fmi2Reset;
        fmi2DoStepDelegate fmi2DoStep;
        fmi2GetRealDelegate fmi2GetReal;
        fmi2SetRealDelegate fmi2SetReal;
        fmi2GetIntegerDelegate fmi2GetInteger;
        fmi2SetIntegerDelegate fmi2SetInteger;
        fmi2GetBooleanDelegate fmi2GetBoolean;
        fmi2SetBooleanDelegate fmi2SetBoolean;
        fmi2GetStringDelegate fmi2GetString;
        fmi2SetStringDelegate fmi2SetString;

        private Dictionary<string, uint> valueReferences;


        public FMU(string fmuName, string instanceName, bool loggingOn=false)
        {
            var modelDescription = Resources.Load<ModelDescription>(fmuName);

            valueReferences = new Dictionary<string, uint>();

            // collect the value references
            foreach (var variable in modelDescription.modelVariables)
            {
                valueReferences[variable.name] = variable.valueReference;
            }

            var unzipdir = Application.streamingAssetsPath + "/" + modelDescription.modelName;

            var modelIdentifier = modelDescription.coSimulation.modelIdentifier;

            var guid = new string(modelDescription.guid);

            var dllPath = unzipdir + "/binaries/";

            // load the DLL
#if UNITY_STANDALONE_WIN
            dllPath += "win" + IntPtr.Size * 8 + "/" + modelIdentifier + ".dll";
            dll = LoadLibrary(dllPath);
#else
            dllPath += "darwin64/" + modelIdentifier + ".dylib";
            dll = dlopen(dllPath, RTLD_NOW);
#endif

            fmi2CallbackFunctions functions;

			functions.logger = new fmi2CallbackLogger(logMessage);
			functions.allocateMemory = new fmi2CallbackAllocateMemory(allocateMemory);
			functions.freeMemory = new fmi2CallbackFreeMemory(free);
			functions.stepFinished = new fmi2StepFinished(stepFinished);
			functions.componentEnvironment = IntPtr.Zero;

			this.callbacks = Marshal.AllocHGlobal(Marshal.SizeOf(functions));
			
            // Copy the struct to unmanaged memory.
			Marshal.StructureToPtr(functions, this.callbacks, false);

			fmi2Instantiate = getFunc<fmi2InstantiateDelegate>(dll, "fmi2Instantiate");
			fmi2FreeInstance = getFunc<fmi2FreeInstanceDelegate>(dll, "fmi2FreeInstance");
			fmi2SetupExperiment = getFunc<fmi2SetupExperimentDelegate>(dll, "fmi2SetupExperiment");
			fmi2EnterInitializationMode = getFunc<fmi2EnterInitializationModeDelegate>(dll, "fmi2EnterInitializationMode");
			fmi2ExitInitializationMode = getFunc<fmi2ExitInitializationModeDelegate>(dll, "fmi2ExitInitializationMode");
			fmi2Terminate = getFunc<fmi2TerminateDelegate>(dll, "fmi2Terminate");
			fmi2Reset = getFunc<fmi2ResetDelegate>(dll, "fmi2Reset");
            fmi2DoStep = getFunc<fmi2DoStepDelegate>(dll, "fmi2DoStep");
            fmi2GetReal = getFunc<fmi2GetRealDelegate>(dll, "fmi2GetReal");
            fmi2SetReal = getFunc<fmi2SetRealDelegate>(dll, "fmi2SetReal");
            fmi2GetInteger = getFunc<fmi2GetIntegerDelegate>(dll, "fmi2GetInteger");
            fmi2SetInteger = getFunc<fmi2SetIntegerDelegate>(dll, "fmi2SetInteger");
            fmi2GetBoolean = getFunc<fmi2GetBooleanDelegate>(dll, "fmi2GetBoolean");
            fmi2SetBoolean = getFunc<fmi2SetBooleanDelegate>(dll, "fmi2SetBoolean");
            fmi2GetString = getFunc<fmi2GetStringDelegate>(dll, "fmi2GetString");
            fmi2SetString = getFunc<fmi2SetStringDelegate>(dll, "fmi2SetString");

            var resourceLocation = new Uri(unzipdir).AbsoluteUri;

            component = fmi2Instantiate(instanceName, (int)fmi2Type.fmi2CoSimulation, guid, resourceLocation, callbacks, fmi2False, loggingOn ? fmi2True : fmi2False);
		}

        public uint GetValueReference(string variable) {
            return valueReferences[variable];
        }

		public void Dispose() {
			Marshal.FreeHGlobal(callbacks);

#if UNITY_STANDALONE_WIN
            FreeLibrary(dll);
#else
			dlclose(dll);
#endif
        }

		public void SetupExperiment(double startTime, double? tolerance = null, double? stopTime = null) {
			var status = fmi2SetupExperiment(component, 
				tolerance.HasValue ? fmi2True : fmi2False, 
				tolerance.HasValue ? tolerance.Value : 0.0, 
				startTime, 
				stopTime.HasValue ? fmi2True : fmi2False, 
				stopTime.HasValue ? stopTime.Value : 0.0);
		}

		public void EnterInitializationMode() {
			var status = fmi2EnterInitializationMode(component);
		}

		public void ExitInitializationMode() {
			var status = fmi2ExitInitializationMode(component);
		}

		public void Terminate() {
			var status = fmi2Terminate(component);
		}

		public void Reset() {
			var status = fmi2Reset(component);
		}

		public void FreeInstance() {
			fmi2FreeInstance(component);
		}

		public void DoStep(double currentCommunicationPoint,
			double communicationStepSize,
			bool noSetFMUStatePriorToCurrentPoint = true) {

			var status = fmi2DoStep(component,
				currentCommunicationPoint,
				communicationStepSize,
				noSetFMUStatePriorToCurrentPoint ? fmi2True : fmi2False);
		}

		public double GetReal(uint vr)
		{
			fmi2ValueReference[] vrs = { vr };
            fmi2Real[] value = { 0.0 };
			var status = fmi2GetReal(component, vrs, 1, value);
			return value[0];
		}

        public double GetReal(string name)
        {
            var vr = valueReferences[name];
            return GetReal(vr);
        }

        public void SetReal(uint vr, double value)
		{
			fmi2ValueReference[] vrs = { vr };
            fmi2Real[] value_ = { value };
			var status = fmi2SetReal(component, vrs, 1, value_);
		}

        public void SetReal(string name, double value)
        {
            var vr = valueReferences[name];
            SetReal(vr, value);
        }

        public int GetInteger(uint vr)
        {
            fmi2ValueReference[] vrs = { vr };
            int[] value = { 0 };
            var status = fmi2GetInteger(component, vrs, 1, value);
            return value[0];
        }

        public int GetInteger(string name)
        {
            var vr = valueReferences[name];
            return GetInteger(vr);
        }

        public void SetInteger(uint vr, int value)
        {
            fmi2ValueReference[] vrs = { vr };
            fmi2Integer[] value_ = { value };
            var status = fmi2SetInteger(component, vrs, 1, value_);
        }

        public void SetInteger(string name, int value)
        {
            var vr = valueReferences[name];
            SetInteger(vr, value);
        }

        public bool GetBoolean(uint vr)
        {
            fmi2ValueReference[] vrs = { vr };
            fmi2Boolean[] value = { fmi2False };
            var status = fmi2GetBoolean(component, vrs, 1, value);
            return value[0] != fmi2False;
        }

        public bool GetBoolean(string name)
        {
            var vr = valueReferences[name];
            return GetBoolean(vr);
        }

        public void SetBoolean(uint vr, bool value)
        {
            fmi2ValueReference[] vrs = { vr };
            fmi2Boolean[] value_ = { value ? fmi2True : fmi2False };
            var status = fmi2SetBoolean(component, vrs, 1, value_);
        }

        public void SetBoolean(string name, bool value)
        {
            var vr = valueReferences[name];
            SetBoolean(vr, value);
        }

        public string GetString(uint vr)
        {
            fmi2ValueReference[] vrs = { vr };
            IntPtr[] value = { IntPtr.Zero };
            var status = fmi2GetString(component, vrs, 1, value);
            var str = Marshal.PtrToStringAnsi(value[0]);
            return str;
        }

        public string GetString(string name)
        {
            var vr = valueReferences[name];
            return GetString(vr);
        }

        public void SetString(uint vr, string value)
        {
            fmi2ValueReference[] vrs = { vr };
            fmi2String[] value_ = { value };
            var status = fmi2SetString(component, vrs, 1, value_);
        }

        public void SetString(string name, string value)
        {
            var vr = valueReferences[name];
            SetString(vr, value);
        }

		static IntPtr allocateMemory(size_t nobj, size_t size)
		{
			//Debug.Log("allocateMemory(" + nobj + ", " + size + ")");

            var nbytes = (int)(nobj * size);

            // allocate the memory
            var mem = Marshal.AllocHGlobal(nbytes);

            var zero = new byte[nbytes];
            
            // set all bytes to 0
            Marshal.Copy(zero, 0, mem, nbytes);

            return mem;
		}

#if UNITY_STANDALONE_WIN
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);
#else
        [DllImport("libdl")]
		protected static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libdl")]
		protected static extern int dlclose(IntPtr handle);

		[DllImport("libdl")]
		private static extern IntPtr dlsym(IntPtr handle, String symbol);
#endif

#if UNITY_STANDALONE_WIN
        [DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl)]
#else
        [DllImport("libc")]
#endif
        static extern void free(IntPtr ptr);

#if UNITY_STANDALONE_WIN
        [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
#else
        [DllImport("libc")]
#endif
        static extern IntPtr calloc(size_t num, size_t size);

		static void stepFinished(fmi2ComponentEnvironment mem, fmi2Status status)
		{

		}

		static void logMessage(IntPtr env, IntPtr instanceName, fmi2Status status, IntPtr category, IntPtr message)
		{
            Debug.Log(Marshal.PtrToStringAnsi(message) + ": " + Marshal.PtrToStringAnsi(message));
		}

		TDelegate getFunc<TDelegate>(IntPtr dll, string fname) where TDelegate : class
		{

#if UNITY_STANDALONE_WIN
            IntPtr p = GetProcAddress(dll, fname);
#else
            IntPtr p = dlsym(dll, fname);
#endif

            if (p == IntPtr.Zero)
			{
				return null;
			}

			Delegate function = Marshal.GetDelegateForFunctionPointer(p, typeof(TDelegate));

			// Ideally, we'd just make the constraint on TDelegate be
			// System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
			// prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
			object o = function;

			return (TDelegate)o;
		}

	}

}
