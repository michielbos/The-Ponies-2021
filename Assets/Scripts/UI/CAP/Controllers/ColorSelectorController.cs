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
    internal class ColorSelectorController : MonoBehaviour
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
        [SerializeField]
        private Text _headerText;

        /// <summary>
        /// The text used to display the Hex value of the color.
        /// </summary>
        [SerializeField]
        private Text _colorHexValueText;
    }
}
