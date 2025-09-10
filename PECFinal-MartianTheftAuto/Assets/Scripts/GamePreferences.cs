using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class GamePreferences : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;

    private void Start()
    {
        volumeSlider.value = AudioListener.volume;
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 2000); ;
    }

    // Function to change the master volume
    public void ChangeMasterVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    // Function to change the brightness
    public void ChangeBrightness()
    {
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        PlayerPrefs.Save();
    }

    // Function to change the contrast
    public void ChangeContrast()
    {
        float contrastValue = 1f + brightnessSlider.value;
        RenderSettings.ambientSkyColor *= contrastValue;
    }

}
