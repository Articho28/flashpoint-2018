using UnityEngine;
using System.Collections;

public class Brightness : MonoBehaviour
{

    public float GammaCorrection;

    public Rect SliderLocation;

    void Update()
    {

        RenderSettings.ambientLight = new Color(GammaCorrection, GammaCorrection, GammaCorrection, 1.0f);

    }

    public void brightnessSlider()
    {

        GammaCorrection = GUI.HorizontalSlider(SliderLocation, GammaCorrection, 0, 1.0f);

    }

}
