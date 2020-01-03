using Assets.Scripts.Util;
using System.Collections.Generic;
using UI.Utils.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CAP
{
    /// <summary>
    /// All the color selection modes available.
    /// </summary>
    internal enum ColorSelectionMode
    {
        Swatches,
        Wheel
    };

    /// <summary>
    /// Controls the Color Selector Panel.
    /// </summary>
    internal class ColorSelectorController : SingletonMonoBehaviour<ColorSelectorController>
    {
        /// <summary>
        /// Tells if the controller is active.
        /// </summary>
        internal bool isActive = false;

        /// <summary>
        /// The color selection mode for the color selector.
        /// </summary>
        internal ColorSelectionMode ColorSelectionMode;

        /// <summary>
        /// The processed color.
        /// </summary>
        internal Color processedColor;

        /// <summary>
        /// The text used as a header.
        /// </summary>
        [Header("Text Values")]
        [SerializeField]
        private Text _headerText;

        /// <summary>
        /// The image that shows the preview of the selected image.
        /// </summary>
        [Header("Preview Color")]
        [SerializeField]
        private Image _previewColorImage = null;

        /// <summary>
        /// The text used to display the Hex value of the color.
        /// </summary>
        [SerializeField]
        private Text _colorHexValueText;

        /// <summary>
        /// The prefab to use to make a swatch color button
        /// </summary>
        [Header("Swatches Color Parameters")]
        [SerializeField]
        private GameObject _colorSwatchButtonPrefab = null;

        /// <summary>
        /// The rect transform holding the grid of the color swatches  
        /// </summary>
        [SerializeField]
        private RectTransform _colorSwatchGridRectTransform = null;

        /// <summary>
        /// List of the colors to generate on the color swatches pannel.
        /// </summary>
        [SerializeField]
        private List<Color> _swatchesColorToGenerate = new List<Color>();

        /// <summary>
        /// Generates all the color swatches buttons.
        /// </summary>
        private void GenerateSwatchesColorButtons()
        {
            GameObject colorSwatch;
            foreach(Color previewColor in _swatchesColorToGenerate)
            {
                colorSwatch = Instantiate<GameObject>(_colorSwatchButtonPrefab, _colorSwatchGridRectTransform);
                colorSwatch.GetComponent<Image>().color = previewColor;
            }
        }

        /// <summary>
        /// Updates the color preview image and the hex value
        /// </summary>
        /// <param name="newColor">The color to display in the preview.</param>
        internal void UpdatePreviewColor(Color newColor)
        {
            if (_previewColorImage != null)
                _previewColorImage.color = newColor;
            if (_colorHexValueText != null)
                _colorHexValueText.text = ("#" + ColorConverter.RGBToHex(newColor).hexValue);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private new void Awake()
        {
            base.Awake();
            GenerateSwatchesColorButtons();
        }
    }
}
