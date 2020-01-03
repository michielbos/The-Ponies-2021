using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace UI.CAP
{
    /// <summary>
    /// The association of a button background and a CAP Category.
    /// </summary>
    [System.Serializable]
    internal struct CAPCategoryButtonBackground
    {
        // Using this pragma to ignore the "is never assigned to, and will always have its default value" because of the struct nature of the element.
        #pragma warning disable 0649
        /// <summary>
        /// The category concerned by the <see cref="associatedBackground"/>.
        /// </summary>
        [SerializeField]
        internal CAPCategories category;

        /// <summary>
        /// The background associated to the category that should be displayed on screen.
        /// </summary>
        [SerializeField]
        internal Sprite associatedBackground;
        #pragma warning restore 0649
    }

    /// <summary>
    /// Controls the modification of the background image of the CAP categories buttons.
    /// </summary>
    public class ButtonBackgroundController : MonoBehaviour
    {
        /// <summary>
        /// The image holding the background of the buttons.
        /// </summary>
        [SerializeField]
        private Image _backgroundHolder = null;

        /// <summary>
        /// All the backgrounds and the associated categories.
        /// </summary>
        [SerializeField]
        private List<CAPCategoryButtonBackground> _buttonBackgrounds = new List<CAPCategoryButtonBackground>();

        /// <summary>
        /// Changes the background according to the category given.
        /// </summary>
        /// <param name="category">Type of the category to switch to.</param>
        private void ChangeBackground(CAPCategories category)
        {
            foreach(CAPCategoryButtonBackground bb in _buttonBackgrounds)
            {
                if(bb.category == category)
                {
                    _backgroundHolder.sprite = bb.associatedBackground;
                    break;
                }
            }
        }

        /// <summary>
        /// Switches the category to Create Pony.
        /// </summary>
        public void SetCreatePonyButtonBackground()
        {
            ChangeBackground(CAPCategories.CreatePony);
        }

        /// <summary>
        /// Switches the category to Head Modification.
        /// </summary>
        public void SetHeadButtonBackground()
        {
            ChangeBackground(CAPCategories.Head);
        }

        /// <summary>
        /// Switches the category to Body Modification.
        /// </summary>
        public void SetBodyButtonBackground()
        {
            ChangeBackground(CAPCategories.Body);
        }

        /// <summary>
        /// Switches the category to Mane Modification.
        /// </summary>
        public void SetManeButtonBackground()
        {
            ChangeBackground(CAPCategories.Mane);
        }

        /// <summary>
        /// Switches the category to Tail Modification.
        /// </summary>
        public void SetTailButtonBackground()
        {
            ChangeBackground(CAPCategories.Tail);
        }

        /// <summary>
        /// Switches the category to Outfit Modification.
        /// </summary>
        public void SetOutfitButtonBackground()
        {
            ChangeBackground(CAPCategories.Outfit);
        }

        /// <summary>
        /// Switches the category to Accessories Modification.
        /// </summary>
        public void SetAccessoriesButtonBackground()
        {
            ChangeBackground(CAPCategories.Accessories);
        }

        /// <summary>
        /// Switches the category to Personality Modification.
        /// </summary>
        public void SetPersonalityButtonBackground()
        {
            ChangeBackground(CAPCategories.Personality);
        }
    }
}