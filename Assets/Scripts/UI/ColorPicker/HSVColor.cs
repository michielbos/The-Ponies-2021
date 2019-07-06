using UnityEngine;

namespace UI.Utils.ColorPicker
{
    /// <summary>
    /// Represents a color in the HUE model.
    /// </summary>
    [System.Serializable]
    public struct HSVColor
    {
        #region HSV Model
        /// <summary>
        /// Hue value of the color.
        /// </summary>
        [SerializeField]
        [Range(0f, 360f)]
        public float hue;

        /// <summary>
        /// Saturation value of the color.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        public float saturation;

        /// <summary>
        /// Value of the color, ie its brightness 
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        public float value;
        #endregion
    }
}
