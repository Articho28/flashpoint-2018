using UnityEngine;
using UnityEngine.UI;
public class Brightness : MonoBehaviour
{

    public Color ambientDarkest = Color.black;

    public Color ambientLightest = Color.white;

    public Slider slider;

    public float sliderValue;
    
    void Start()
    {
        // Make the ambient lighting red
        RenderSettings.ambientLight = ambientDarkest;
    }

    void Update()
    {
        slider = GameObject.Find("BrightnessSlider").GetComponent<Slider>();
        sliderValue = GameObject.Find("BrightnessSlider").GetComponent<Slider>().value;
        slider.value = sliderValue;
        ChangeBrightness(sliderValue);

    }

    public void ChangeBrightness(float value){
        RenderSettings.ambientLight = Color.Lerp(ambientDarkest, ambientLightest, value);
    }

}
