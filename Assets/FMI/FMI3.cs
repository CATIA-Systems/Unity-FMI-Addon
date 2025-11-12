using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

#pragma warning disable IDE1006 // Naming Styles
namespace FMI3 {

	using size_t = UInt64;
	using fmi3String = String;
	using fmi3InstanceEnvironment = IntPtr;
    using fmi3Float64 = Double;
    using fmi3Int32 = Int32;
	using fmi3Boolean = Boolean;
	using fmi3Instance = IntPtr;
	using fmi3ValueReference = UInt32;
    using fmi3IntermediateUpdateCallback = IntPtr;

    public enum fmi3Status
    {
        fmi3OK,
        fmi3Warning,
        fmi3Discard,
        fmi3Error,
        fmi3Fatal
    };

    // typedef void  (*fmi3LogMessageCallback) (fmi3InstanceEnvironment instanceEnvironment,
    //                                          fmi3Status status,
    //                                          fmi3String category,
    //                                          fmi3String message);
	public delegate void fmi3LogMessageCallback(
        fmi3InstanceEnvironment instanceEnvironment,
        fmi3Status status,
        fmi3String category,
        fmi3String message
    );

    /* Creation and destruction of FMU instances and setting debug status */
    // typedef fmi3Instance fmi3InstantiateCoSimulationTYPE(
    //     fmi3String                     instanceName,
    //     fmi3String                     instantiationToken,
    //     fmi3String                     resourcePath,
    //     fmi3Boolean                    visible,
    //     fmi3Boolean                    loggingOn,
    //     fmi3Boolean                    eventModeUsed,
    //     fmi3Boolean                    earlyReturnAllowed,
    //     const fmi3ValueReference       requiredIntermediateVariables[],
    //     size_t                         nRequiredIntermediateVariables,
    //     fmi3InstanceEnvironment        instanceEnvironment,
    //     fmi3LogMessageCallback         logMessage,
    //     fmi3IntermediateUpdateCallback intermediateUpdate);
    delegate fmi3Instance fmi3InstantiateCoSimulationDelegate(
        fmi3String                     instanceName,
        fmi3String                     instantiationToken,
        fmi3String                     resourcePath,
        fmi3Boolean                    visible,
        fmi3Boolean                    loggingOn,
        fmi3Boolean                    eventModeUsed,
        fmi3Boolean                    earlyReturnAllowed,
        fmi3ValueReference[]           requiredIntermediateVariables,
        size_t                         nRequiredIntermediateVariables,
        fmi3InstanceEnvironment        instanceEnvironment,
        fmi3LogMessageCallback         logMessage,
        fmi3IntermediateUpdateCallback intermediateUpdate
    );

    // typedef void fmi3FreeInstanceTYPE(fmi3Instance instance);
	delegate void fmi3FreeInstanceDelegate(fmi3Instance instance);

    /* Enter and exit initialization mode, enter event mode, terminate and reset */

    // typedef fmi3Status fmi3EnterInitializationModeTYPE(fmi3Instance instance,
    //                                                    fmi3Boolean toleranceDefined,
    //                                                    fmi3Float64 tolerance,
    //                                                    fmi3Float64 startTime,
    //                                                    fmi3Boolean stopTimeDefined,
    //                                                    fmi3Float64 stopTime);
    delegate fmi3Status fmi3EnterInitializationModeDelegate(
        fmi3Instance instance,
        fmi3Boolean toleranceDefined,
        fmi3Float64 tolerance,
        fmi3Float64 startTime,
        fmi3Boolean stopTimeDefined,
        fmi3Float64 stopTime);


    // typedef fmi3Status fmi3ExitInitializationModeTYPE(fmi3Instance instance);
    delegate fmi3Status fmi3ExitInitializationModeDelegate(fmi3Instance instance);

	// typedef fmi3Status fmi3TerminateTYPE(fmi3Instance instance);
	delegate fmi3Status fmi3TerminateDelegate(fmi3Instance instance);

	// typedef fmi3Status fmi3ResetTYPE(fmi3Instance instance);
	delegate fmi3Status fmi3ResetDelegate(fmi3Instance instance);

    /* Getting and setting variable values */

    // typedef fmi3Status fmi3GetFloat64TYPE(fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       fmi3Float64 values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3GetFloat64Delegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3Float64[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3GetInt32TYPE  (fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       fmi3Int32 values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3GetInt32Delegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3Int32[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3GetBooleanTYPE(fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       fmi3Boolean values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3GetBooleanDelegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3Boolean[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3GetStringTYPE (fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       fmi3String values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3GetStringDelegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3String[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3SetFloat64TYPE(fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       const fmi3Float64 values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3SetFloat64Delegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3Float64[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3SetInt32TYPE  (fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       const fmi3Int32 values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3SetInt32Delegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3Int32[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3SetBooleanTYPE(fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       const fmi3Boolean values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3SetBooleanDelegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3Boolean[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3SetStringTYPE (fmi3Instance instance,
    //                                       const fmi3ValueReference valueReferences[],
    //                                       size_t nValueReferences,
    //                                       const fmi3String values[],
    //                                       size_t nValues);
    delegate fmi3Status fmi3SetStringDelegate(
        fmi3Instance instance,
        fmi3ValueReference[] valueReferences,
        size_t nValueReferences,
        fmi3String[] values,
        size_t nValues
    );

    // typedef fmi3Status fmi3DoStepTYPE(fmi3Instance instance,
    //                                   fmi3Float64 currentCommunicationPoint,
    //                                   fmi3Float64 communicationStepSize,
    //                                   fmi3Boolean noSetFMUStatePriorToCurrentPoint,
    //                                   fmi3Boolean* eventHandlingNeeded,
    //                                   fmi3Boolean* terminateSimulation,
    //                                   fmi3Boolean* earlyReturn,
    //                                   fmi3Float64* lastSuccessfulTime);
    delegate fmi3Status fmi3DoStepDelegate(
        fmi3Instance instance,
        fmi3Float64 currentCommunicationPoint,
        fmi3Float64 communicationStepSize,
        fmi3Boolean noSetFMUStatePriorToCurrentPoint,
        ref fmi3Boolean eventHandlingNeeded,
        ref fmi3Boolean terminateSimulation,
        ref fmi3Boolean earlyReturn,
        ref fmi3Float64 lastSuccessfulTime
    );

    public class FMU : FMI.FMU, IDisposable
    {
        // const fmi3Boolean fmi3True = true;
        // const fmi3Boolean fmi3False = false;

        // IntPtr dll;
        private fmi3Instance instance;

        readonly fmi3InstantiateCoSimulationDelegate fmi3InstantiateCoSimulation;
        readonly fmi3FreeInstanceDelegate fmi3FreeInstance;
        readonly fmi3EnterInitializationModeDelegate fmi3EnterInitializationMode;
        readonly fmi3ExitInitializationModeDelegate fmi3ExitInitializationMode;
        readonly fmi3TerminateDelegate fmi3Terminate;
        readonly fmi3ResetDelegate fmi3Reset;
        readonly fmi3DoStepDelegate fmi3DoStep;
        readonly fmi3GetFloat64Delegate fmi3GetFloat64;
        // readonly fmi3GetBooleanDelegate fmi3GetBoolean;
        // readonly fmi3SetBooleanDelegate fmi3SetBoolean;
        // readonly fmi3GetStringDelegate fmi3GetString;
        readonly fmi3SetFloat64Delegate fmi3SetFloat64;
        readonly fmi3GetInt32Delegate fmi3GetInt32;
        readonly fmi3SetInt32Delegate fmi3SetInt32;
        // readonly fmi3SetStringDelegate fmi3SetString;

        private Dictionary<string, uint> valueReferences;

        public FMU(string fmuName, string instanceName, bool loggingOn = false)
        {
            var modelDescription = Resources.Load<ModelDescription>(fmuName);

            if (!modelDescription.fmiVersion.StartsWith("3."))
            {
                throw new Exception("Model description must be FMI version 3.");
            }

            valueReferences = new Dictionary<string, uint>();

            // collect the value references
            foreach (var variable in modelDescription.modelVariables)
            {
                valueReferences[variable.name] = variable.valueReference;
            }

            var unzipdir = Path.Combine(Application.streamingAssetsPath, fmuName);

            var modelIdentifier = modelDescription.coSimulation.modelIdentifier;

            var guid = modelDescription.instantiationToken;

            // load the DLL
#if UNITY_STANDALONE_WIN
            var dllPath = Path.Combine(unzipdir, "binaries", "x86_64-windows", modelIdentifier + ".dll");
            dll = LoadLibrary(dllPath);
#else
            var dllPath =  Path.Combine(unzipdir, "binaries", "x86_64-darwin", modelIdentifier + ".dylib");
            dll = dlopen(dllPath, RTLD_NOW);
#endif

            if (dll == IntPtr.Zero)
            {
                throw new Exception("Could not load the DLL: " + dllPath);
            }

            fmi3InstantiateCoSimulation = getFunc<fmi3InstantiateCoSimulationDelegate>("fmi3InstantiateCoSimulation");
            fmi3FreeInstance = getFunc<fmi3FreeInstanceDelegate>("fmi3FreeInstance");
            fmi3EnterInitializationMode = getFunc<fmi3EnterInitializationModeDelegate>("fmi3EnterInitializationMode");
            fmi3ExitInitializationMode = getFunc<fmi3ExitInitializationModeDelegate>("fmi3ExitInitializationMode");
            fmi3Terminate = getFunc<fmi3TerminateDelegate>("fmi3Terminate");
            fmi3Reset = getFunc<fmi3ResetDelegate>("fmi3Reset");
            fmi3DoStep = getFunc<fmi3DoStepDelegate>("fmi3DoStep");
            fmi3GetFloat64 = getFunc<fmi3GetFloat64Delegate>("fmi3GetFloat64");
            fmi3SetFloat64 = getFunc<fmi3SetFloat64Delegate>("fmi3SetFloat64");
            fmi3GetInt32 = getFunc<fmi3GetInt32Delegate>("fmi3GetInt32");
            fmi3SetInt32 = getFunc<fmi3SetInt32Delegate>("fmi3SetInt32");
            // fmi3GetInteger = getFunc<fmi3GetIntegerDelegate>("fmi3GetInteger");
            // fmi3SetInteger = getFunc<fmi3SetIntegerDelegate>("fmi3SetInteger");
            // fmi3GetBoolean = getFunc<fmi3GetBooleanDelegate>("fmi3GetBoolean");
            // fmi3SetBoolean = getFunc<fmi3SetBooleanDelegate>("fmi3SetBoolean");
            // fmi3GetString = getFunc<fmi3GetStringDelegate>("fmi3GetString");
            // fmi3SetString = getFunc<fmi3SetStringDelegate>("fmi3SetString");

            var resourceLocation = new Uri(unzipdir).AbsoluteUri;

            fmi3ValueReference[] requiredIntermediateVariables = { };

            instance = fmi3InstantiateCoSimulation(instanceName, modelDescription.instantiationToken, "", false, false, false, false, requiredIntermediateVariables, 0, IntPtr.Zero, logMessage, IntPtr.Zero);
            
            if (instance == IntPtr.Zero)
            {
                throw new Exception("Could not instantiate the FMU.");
            }
        }

        ~FMU()
        {
            Debug.Log("~FMU()");
#if UNITY_STANDALONE_WIN
            FreeLibrary(dll);
#else
			dlclose(dll);
#endif
        }

        public void Dispose()
        {
            fmi3FreeInstance(instance);
            instance = IntPtr.Zero;
        }

        public uint GetValueReference(string variable)
        {
            return valueReferences[variable];
        }

        private void Call(fmi3Status status)
        {
            if (status != fmi3Status.fmi3OK)
            {
                throw new Exception("FMI call failed.");
            }
        }

        public void EnterInitializationMode(double startTime, double? tolerance = null, double? stopTime = null)
        {
            Call(fmi3EnterInitializationMode(
                instance,
                tolerance.HasValue,
                tolerance ?? 0.0,
                startTime,
                stopTime.HasValue,
                stopTime ?? 0.0));

            if (LogFMICalls)
            {
                Debug.Log("fmi3EnterInitializationMode(" +
                    "toleranceDefined=" + tolerance.HasValue +
                    ", tolerance=" + (tolerance ?? 0.0) +
                    ", startTime=" + startTime +
                    ", stopTimeDefined=" + stopTime.HasValue +
                    ", stopTime=" + (stopTime ?? 0.0) +
                    ")");
            }
        }

        public void ExitInitializationMode()
        {
            Call(fmi3ExitInitializationMode(instance));

            if (LogFMICalls)
            {
                Debug.Log("fmi3ExitInitializationMode()");
            }
        }

        public void Terminate()
        {
            Call(fmi3Terminate(instance));

            if (LogFMICalls)
            {
                Debug.Log("fmi3Terminate()");
            }
        }

        public void Reset()
        {
            Call(fmi3Reset(instance));

            if (LogFMICalls)
            {
                Debug.Log("fmi3Reset()");
            }
        }

        public void DoStep(double currentCommunicationPoint,
            double communicationStepSize,
            bool noSetFMUStatePriorToCurrentPoint = true)
        {
            fmi3Boolean eventHandlingNeeded = false;
            fmi3Boolean terminateSimulation = false;
            fmi3Boolean earlyReturn = false;
            fmi3Float64 lastSuccessfulTime = 0.0;

            Call(fmi3DoStep(
                instance,
                currentCommunicationPoint,
                communicationStepSize,
                noSetFMUStatePriorToCurrentPoint,
                ref eventHandlingNeeded,
                ref terminateSimulation,
                ref earlyReturn,
                ref lastSuccessfulTime
            ));

            if (LogFMICalls)
            {
                Debug.Log("fmi3DoStep(" +
                    "currentCommunicationPoint=" + currentCommunicationPoint +
                    ", communicationStepSize=" + communicationStepSize +
                    ", noSetFMUStatePriorToCurrentPoint=" + noSetFMUStatePriorToCurrentPoint +
                    ")");
            }
        }

        public void GetFloat64(fmi3ValueReference[] valueReferences, double[] values)
        {
            Call(fmi3GetFloat64(instance, valueReferences, 1, values, (ulong)values.Length));

            if (LogFMICalls)
            {
                Debug.Log("fmi3GetFloat64(" +
                    "valueReferences=[" + string.Join(", ", valueReferences) + "]" +
                    ", values=[" + string.Join(", ", values) + "]" +
                    ")");
            }
        }

        public T GetValue<T>(string name)
        {
            if (typeof(T) == typeof(float))
            {
                double[] values = new double[1];
                GetFloat64(name, values);
                return (T)(object)(float)values[0];
            }
            else if (typeof(T) == typeof(int))
            {
                int[] values = new int[1];
                GetInt32(name, values);
                return (T)(object)values[0];
            }
            else if (typeof(T) == typeof(Vector3))
            {
                double[] values = new double[3];
                GetFloat64(name, values);
                return (T)(object)new Vector3(
                    (float)values[0],
                    (float)values[1],
                    (float)values[2]
                );
            }
            else if (typeof(T) == typeof(Quaternion))
            {
                double[] values = new double[3];
                GetFloat64(name, values);
                return (T)(object)Quaternion.Euler(
                    (float)values[0],
                    (float)values[1],
                    (float)values[2]
                );
            }
            else
            {
                throw new Exception("Unsupported type in GetVariable<" + typeof(T) + "> for variable " + name);
            }
        }

        public void GetFloat64(string name, double[] values)
        {
            GetFloat64(new fmi3ValueReference[] { valueReferences[name] }, values);
        }

        public void GetInt32(uint vr, int[] values)
        {
            fmi3ValueReference[] vrs = { vr };
            Call(fmi3GetInt32(instance, vrs, 1, values, (ulong)values.Length));
        }

        public void GetInt32(string name, int[] values)
        {
            var vr = valueReferences[name];
            GetInt32(vr, values);
        }

        public void SetFloat64(uint vr, double[] values)
        {
            fmi3ValueReference[] vrs = { vr };
            Call(fmi3SetFloat64(instance, vrs, 1, values, (size_t)values.Length));
        }

        public void SetFloat64(string name, double[] values)
        {
            var vr = valueReferences[name];
            SetFloat64(vr, values);
        }

        public void SetInt32(uint vr, int[] values)
        {
            fmi3ValueReference[] vrs = { vr };
            Call(fmi3SetInt32(instance, vrs, 1, values, (size_t)values.Length));
        }

        public void SetInt32(string name, int[] values)
        {
            var vr = valueReferences[name];
            SetInt32(vr, values);
        }

        public void SetValue<T>(string name, T value)
        {
            var vr = valueReferences[name];

            if (typeof(T) == typeof(float))
            {
                SetFloat64(vr, new double[] { (float)(object)value });
            }
            else if (typeof(T) == typeof(int))
            {
                SetInt32(name, new int[] { (int)(object)value });
            }
            else
            {
                throw new Exception("Unsupported type in SetValue<" + typeof(T) + ">");
            }
        }

        // public int GetInteger(uint vr)
        // {
        //     fmi3ValueReference[] vrs = { vr };
        //     int[] value = { 0 };
        //     Call(fmi3GetInteger(instance, vrs, 1, value));
        //     return value[0];
        // }

        // public int GetInteger(string name)
        // {
        //     var vr = valueReferences[name];
        //     return GetInteger(vr);
        // }

        // public void SetInteger(uint vr, int value)
        // {
        //     fmi3ValueReference[] vrs = { vr };
        //     fmi3Integer[] value_ = { value };
        //     Call(fmi3SetInteger(instance, vrs, 1, value_));
        // }

        // public void SetInteger(string name, int value)
        // {
        //     var vr = valueReferences[name];
        //     SetInteger(vr, value);
        // }

        // public bool GetBoolean(uint vr)
        // {
        //     fmi3ValueReference[] vrs = { vr };
        //     fmi3Boolean[] value = { fmi3False };
        //     Call(fmi3GetBoolean(instance, vrs, 1, value));
        //     return value[0] != fmi3False;
        // }

        // public bool GetBoolean(string name)
        // {
        //     var vr = valueReferences[name];
        //     return GetBoolean(vr);
        // }

        // public void SetBoolean(uint vr, bool value)
        // {
        //     fmi3ValueReference[] vrs = { vr };
        //     fmi3Boolean[] value_ = { value ? fmi3True : fmi3False };
        //     Call(fmi3SetBoolean(instance, vrs, 1, value_));
        // }

        // public void SetBoolean(string name, bool value)
        // {
        //     var vr = valueReferences[name];
        //     SetBoolean(vr, value);
        // }

        // public string GetString(uint vr)
        // {
        //     fmi3ValueReference[] vrs = { vr };
        //     IntPtr[] value = { IntPtr.Zero };
        //     Call(fmi3GetString(instance, vrs, 1, value));
        //     var str = Marshal.PtrToStringAnsi(value[0]);
        //     return str;
        // }

        // public string GetString(string name)
        // {
        //     var vr = valueReferences[name];
        //     return GetString(vr);
        // }

        // public void SetString(uint vr, string value)
        // {
        //     fmi3ValueReference[] vrs = { vr };
        //     fmi3String[] value_ = { value };
        //     Call(fmi3SetString(instance, vrs, 1, value_));
        // }

        // public void SetString(string name, string value)
        // {
        //     var vr = valueReferences[name];
        //     SetString(vr, value);
        // }

        static void logMessage(
            fmi3InstanceEnvironment instanceEnvironment,
            fmi3Status status,
            fmi3String category,
            fmi3String message
        )
        {
            switch (status)
            {
                case fmi3Status.fmi3OK:
                    Debug.Log(message);
                    break;
                case fmi3Status.fmi3Warning:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.LogError(message);
                    break;
            }
        }

    }

}
#pragma warning restore IDE1006 // Naming Styles
