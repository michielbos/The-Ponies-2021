using UnityEngine;

namespace UI.Utils.ColorPicker
{
    /// <summary>
    /// Represents a color in the HEX model.
    /// </summary>
    [System.Serializable]
    public struct HexColor
    {
        #region Hex Model
        /// <summary>
        /// The hex value of the color.
        /// </summary>
        [SerializeField]
        public string hexValue;
        #endregion
    }
}