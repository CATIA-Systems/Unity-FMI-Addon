using UnityEngine;
using UnityEngine.InputSystem;
using FMI3;


public class Car : MonoBehaviour
{
    FMU fmu;

    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;

    public float acceleratorDemand;
    public float brakeDemand;
    public float steeringDemand;

    public float engineSpeed;
    public int currentGear;

    InputAction steerAction;
    InputAction accelerateAction;
    InputAction brakeAction;

    double time;  // time of the FMU

    void Start()
    {
        accelerateAction = InputSystem.actions.FindAction("Accelerate");
        brakeAction = InputSystem.actions.FindAction("Brake");
        steerAction = InputSystem.actions.FindAction("Steer");

        fmu = new FMU("LockToLockFigureEight", name);
        
        // fmu.LogFMICalls = true;

        Reset();
    }

    void FixedUpdate()
    {
        if (fmu == null) return;

        transform.position = fmu.GetValue<Vector3>("animationBus.chassisPosition");
        transform.rotation = fmu.GetValue<Quaternion>("animationBus.chassisRotation");

        acceleratorDemand = accelerateAction.ReadValue<float>();
        brakeDemand = brakeAction.ReadValue<float>();
        steeringDemand = steerAction.ReadValue<float>();

        fmu.SetValue("driverInterface.acceleratorDemand", acceleratorDemand);
        fmu.SetValue("driverInterface.brakeDemand", brakeDemand);
        fmu.SetValue("driverInterface.steeringDemand", steeringDemand);

        double h = Time.time - time;

        fmu.DoStep(time, h);

        time = Time.time;

        double[] angle = new double[1];

        wheelFL.localPosition = fmu.GetValue<Vector3>("animationBus.wheelFLPosition");
        wheelFL.localRotation = fmu.GetValue<Quaternion>("animationBus.wheelFLRotation");
        wheelFL.RotateAround(wheelFL.position, wheelFL.forward, fmu.GetValue<float>("animationBus.wheelFLAngle"));

        wheelFR.localPosition = fmu.GetValue<Vector3>("animationBus.wheelFRPosition");
        wheelFR.localRotation = fmu.GetValue<Quaternion>("animationBus.wheelFRRotation");
        wheelFR.RotateAround(wheelFR.position, wheelFR.forward, fmu.GetValue<float>("animationBus.wheelFRAngle"));

        wheelRL.localPosition = fmu.GetValue<Vector3>("animationBus.wheelRLPosition");
        wheelRL.localRotation = fmu.GetValue<Quaternion>("animationBus.wheelRLRotation");
        wheelRL.RotateAround(wheelRL.position, wheelRL.forward, fmu.GetValue<float>("animationBus.wheelRLAngle"));

        wheelRR.localPosition = fmu.GetValue<Vector3>("animationBus.wheelRRPosition");
        wheelRR.localRotation = fmu.GetValue<Quaternion>("animationBus.wheelRRRotation");
        wheelRR.RotateAround(wheelRR.position, wheelRR.forward, fmu.GetValue<float>("animationBus.wheelRRAngle"));

        engineSpeed = fmu.GetValue<float>("driverInterface.engineSpeed");
        currentGear = fmu.GetValue<int>("driverInterface.currentGear");
    }

    public void Reset()
    {
        if (fmu == null) return;

        fmu.Reset();

        time = Time.time;
        
        fmu.EnterInitializationMode(time);
        fmu.ExitInitializationMode();
        
        fmu.SetValue("driverInterface.ignition", 2);
        fmu.SetValue("driverInterface.gear", 2);
    }

}
