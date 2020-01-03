using UnityEngine;
using UnityEngine.UI;

namespace UI.CAP
{
    /// <summary>
    /// The behaviour of a color swatch button.
    /// </summary>
    internal class ColorSwatchButtonBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The image showed by the button.
        /// </summary>
        [SerializeField]
        private Image _buttonImage;

        /// <summary>
        /// Calls for an update the color preview displayed by the Color Selector Controller.
        /// </summary>
        public void UpdateColorPreview()
        {
            ColorSelectorController.GetInstance().UpdatePreviewColor(_buttonImage.color);
        }
    }
}