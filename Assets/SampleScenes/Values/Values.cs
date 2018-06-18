using UnityEngine;
using UnityEngine.UI;
using FMI2;


public class Values : MonoBehaviour
{

    public Slider realInSlider;

    public Text realOutText;

    public Slider intInSlider;

    public Text intOutText;

    public Toggle boolInToggle;

    public Text boolOutText;

    public InputField stringInInputField;

    public Text stringOutText;

    private FMU fmu;

    private uint vr_x, vr_int_in, vr_bool_in, vr_string_in;

    void Start()
    {
        // instantiate the FMU
        fmu = new FMU("values", name);

        // reset & sync the UI
        Reset();

        // get the value references
        vr_x = fmu.GetValueReference("x");
        vr_int_in = fmu.GetValueReference("int_in");
        vr_bool_in = fmu.GetValueReference("bool_in");
        vr_string_in = fmu.GetValueReference("string_in");

        // listen to changes in the UI
        realInSlider.onValueChanged.AddListener(SetX);
        intInSlider.onValueChanged.AddListener(SetIntIn);
        boolInToggle.onValueChanged.AddListener(SetBoolIn);
        stringInInputField.onValueChanged.AddListener(SetStringIn);
    }

    public void Reset()
    {
        // reset the FMU
        fmu.Reset();

        // start the experiment at the current time
        fmu.SetupExperiment(Time.time);

        // initialize
        fmu.EnterInitializationMode();
        fmu.ExitInitializationMode();

        // sync UI
        realInSlider.value = (float)fmu.GetReal(vr_x);
        realOutText.text = realInSlider.value.ToString();

        intInSlider.value = fmu.GetInteger(vr_int_in);
        intOutText.text = intInSlider.value.ToString();

        boolInToggle.isOn = fmu.GetBoolean(vr_bool_in);
        boolOutText.text = boolInToggle.isOn.ToString();

        stringOutText.text = stringInInputField.text = fmu.GetString(vr_string_in);
    }

    void SetX(float x)
    {
        fmu.SetReal(vr_x, x);
        realOutText.text = fmu.GetReal(vr_x).ToString();
    }

    void SetIntIn(float intIn)
    {
        fmu.SetInteger(vr_int_in, (int)intIn);
        intOutText.text = fmu.GetInteger(vr_int_in).ToString();
    }

    void SetBoolIn(bool boolIn)
    {
        fmu.SetBoolean(vr_bool_in, boolIn);
        boolOutText.text = fmu.GetBoolean(vr_bool_in).ToString();
    }

    void SetStringIn(string s)
    {
        stringOutText.text = s;
    }

    void OnDestroy()
    {
        // clean up
        fmu.Dispose();
    }

}
