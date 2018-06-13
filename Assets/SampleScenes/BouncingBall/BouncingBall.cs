using UnityEngine;
using FMI2;


public class BouncingBall : MonoBehaviour {

    private FMU fmu;

    [Range(1, 6)]
    public float initalHeight = 4;

    [Range(0.5f, 0.95f)]
    public float reboundFactor = 0.7f;

	void Start () {

        // instantiate the FMU
        fmu = new FMU("bouncingBall", name);

        Reset();
    }

    public void SetReboundFactor(float e)
    {
        reboundFactor = Mathf.Clamp(e, 0.5f, 0.95f);
        
        // set the variable "e" (rebound factor)
        fmu.SetReal("e", reboundFactor);
    }

    public void Reset()
    {
        // reset the FMU
        fmu.Reset();

        // start the experiment at the current time
        fmu.SetupExperiment(Time.time);

        fmu.EnterInitializationMode();

        // set the start values for variables "h" and "e"
        fmu.SetReal("h", initalHeight);
        fmu.SetReal("e", reboundFactor);

        fmu.ExitInitializationMode();
    }

    void FixedUpdate()
    {
        // synchronize the model with the current time
        fmu.DoStep(Time.time, Time.deltaTime);

        // get the variable "h" (height)
        transform.position = Vector3.up * (float)fmu.GetReal("h");
    }

    void OnDestroy()
    {
        // clean up
        fmu.Dispose();
    }

}
