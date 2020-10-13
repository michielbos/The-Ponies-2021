using UnityEngine;
using UnityEngine.UI;

namespace UI.Utils.ColorPicker
{
    /// <summary>
    /// The controller of the Color Picker UI.
    /// </summary>
    public class ColorPickerUIController : MonoBehaviour
    {
        /// <summary>
        /// This slider is used for the red value of the color.
        /// </summary>
        [SerializeField]
        [Header("Values' Sliders")]
        private Slider _redSlider = null;

        /// <summary>
        /// This slider is used for the green value of the color.
        /// </summary>
        [SerializeField]
        private Slider _greenSlider = null;

        /// <summary>
        /// This slider is used for the blue value of the colors.
        /// </summary>
        [SerializeField]
        private Slider _blueSlider = null;

        /// <summary>
        /// The color processed by the Color Picker as an RGB color. 
        /// </summary>
        public Color rgbColor = new Color(1f, 1f, 1f, 1f);

        /// <summary>
        /// The color processed by the Color Picker as an HSV color. 
        /// Obtained by conversion of the RGB Color.
        /// </summary>
        public HSVColor ColorHSV { get { return ColorConverter.RGBToHSV(rgbColor); } }

        /// <summary>
        /// Updates the red value of the processed color.
        /// </summary>
        /// <param name="newRedValue">The new value for the red canal.</param>
        private void UpdateRedValue(float newRedValue)
        {
            Debug.Log("Updated Red.");
            rgbColor.r = newRedValue;
        }

        /// <summary>
        /// Updates the green value of the processed color.
        /// </summary>
        /// <param name="newGreenValue">The new value for the green canal.</param>
        private void UpdateGreenValue(float newGreenValue)
        {
            Debug.Log("Updated Red.");
            rgbColor.g = newGreenValue;
        }

        /// <summary>
        /// Updates the bleu value of the processed color.
        /// </summary>
        /// <param name="newBlueValue">The new value for the blue canal.</param>
        private void UpdateBlueValue(float newBlueValue)
        {
            Debug.Log("Updated Red.");
            rgbColor.b = newBlueValue;
        }
     
        /// <summary>
        /// Sets the boundaries of the given slider to a normalize value 
        /// </summary>
        /// <param name="slider">The slider to normalize.</param>
        private void NormalizeSliderBoundaries(Slider slider)
        {
            slider.value = (slider.value - slider.minValue) / (slider.maxValue - slider.minValue);
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }

        /// <summary>
        /// Subscribe all the sliders changing value's events on the needed functions.
        /// </summary>
        public void Start()
        {
            if (_redSlider != null)
            {
                NormalizeSliderBoundaries(_redSlider);
                _redSlider.onValueChanged.AddListener(UpdateRedValue);
            }
            if (_greenSlider != null)
            {
                NormalizeSliderBoundaries(_greenSlider);
                _greenSlider.onValueChanged.AddListener(UpdateGreenValue);
            }
            if (_blueSlider != null)
            {
                NormalizeSliderBoundaries(_blueSlider);
                _blueSlider.onValueChanged.AddListener(UpdateBlueValue);
            }
        }

        /// <summary>
        /// Unsubscribe all the sliders changing value's events on the needed functions.
        /// </summary>
        public void OnDestroy()
        {
            if (_redSlider != null)
                _redSlider.onValueChanged.RemoveListener(UpdateRedValue);
            if (_greenSlider != null)
                _greenSlider.onValueChanged.RemoveListener(UpdateGreenValue);
            if (_blueSlider != null)
                _blueSlider.onValueChanged.RemoveListener(UpdateBlueValue);
        }
    }
}
