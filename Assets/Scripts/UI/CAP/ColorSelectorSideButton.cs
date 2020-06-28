using UnityEngine;
using UnityEngine.UI;

namespace UI.CAP
{
    /// <summary>
    /// The button behaviour for the buttons on the Color Selector Side Panel in the Create a Pony UI.
    /// </summary>
    internal class ColorSelectorSideButton : MonoBehaviour
    {
        /// <summary>
        /// The title to display on the Color Selector Panel.
        /// </summary>
        [SerializeField]
        internal string associatedTitle = "Color";

        /// <summary>
        /// The image used to display the selected color.
        /// </summary>
        [SerializeField]
        private Image _colorDisplayer = null;
    }
}
