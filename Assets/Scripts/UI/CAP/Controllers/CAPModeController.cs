using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.CAP
{
    /// <summary>
    /// The association of a button background and a CAP Category.
    /// </summary>
    [System.Serializable]
    internal class CAPCategoriesPanelAssociation
    {
        /// <summary>
        /// The category concerned by the <see cref="associatedBackground"/>.
        /// </summary>
        [SerializeField]
        internal CAPCategories category = CAPCategories.CreatePony;

        /// <summary>
        /// The background associated to the category that should be displayed on screen.
        /// </summary>
        [SerializeField]
        internal Sprite associatedBackground = null;

        /// <summary>
        /// The game object holding the panel of the associated category.
        /// </summary>
        [SerializeField] internal GameObject associatedPanel = null;
    }

    /// <summary>
    /// Controls the active mode in the CAP space
    /// </summary>
    public class CAPModeController : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// The list of the different panels associated to a mode.
        /// </summary>
        [SerializeField]
        private List<CAPCategoriesPanelAssociation> capPanelsList = new List<CAPCategoriesPanelAssociation>();

        /// <summary>
        /// The image holding the background of the buttons.
        /// </summary>
        [SerializeField]
        private Image _backgroundHolder = null;

        #endregion Fields

        #region Events

        /// <summary>
        /// The event raised when the CAP mode is changed.
        /// </summary>
        public event Action<CAPCategories> CapModeChangedEvent;

        #endregion Events

        #region Methods

        /// <summary>
        /// Changes the current mode of the CAP
        /// </summary>
        /// <param name="newCategory">The new category to use.</param>
        private void ChangeCAPMode(CAPCategories newCategory)
        {
            if (CapModeChangedEvent != null)
            {
                CapModeChangedEvent.Invoke(newCategory);
            }
        }

        /// <summary>
        /// Switches the category to Create Pony.
        /// </summary>
        public void SetCreatePonyMode()
        {
            this.ChangeCAPMode(CAPCategories.CreatePony);
        }

        /// <summary>
        /// Switches the category to Head Modification.
        /// </summary>
        public void SetHeadMode()
        {
            this.ChangeCAPMode(CAPCategories.Head);
        }

        /// <summary>
        /// Switches the category to Body Modification.
        /// </summary>
        public void SetBodyMode()
        {
            this.ChangeCAPMode(CAPCategories.Body);
        }

        /// <summary>
        /// Switches the category to Mane Modification.
        /// </summary>
        public void SetManeMode()
        {
            this.ChangeCAPMode(CAPCategories.Mane);
        }

        /// <summary>
        /// Switches the category to Tail Modification.
        /// </summary>
        public void SetTailMode()
        {
            this.ChangeCAPMode(CAPCategories.Tail);
        }

        /// <summary>
        /// Switches the category to Outfit Modification.
        /// </summary>
        public void SetOutfitMode()
        {
            this.ChangeCAPMode(CAPCategories.Outfit);
        }

        /// <summary>
        /// Switches the category to Accessories Modification.
        /// </summary>
        public void SetAccessoriesMode()
        {
            this.ChangeCAPMode(CAPCategories.Accessories);
        }

        /// <summary>
        /// Switches the category to Personality Modification.
        /// </summary>
        public void SetPersonalityMode()
        {
            this.ChangeCAPMode(CAPCategories.Personality);
        }

        /// <summary>
        /// Changes the active mode on the CAP panel
        /// </summary>
        /// <param name="category">The category to set active.</param>
        public void ChangeActiveCAPPanel(CAPCategories category)
        {
            foreach (CAPCategoriesPanelAssociation association in capPanelsList)
            {
                if (association.category == category)
                {
                    _backgroundHolder.sprite = association.associatedBackground;
                    association.associatedPanel.SetActive(true);
                }
                else
                {
                    association.associatedPanel.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Called at the start of the scene
        /// </summary>
        private void Start()
        {
            this.CapModeChangedEvent += this.ChangeActiveCAPPanel;
        }

        /// <summary>
        /// Called when the object is destroy.
        /// </summary>
        private void OnDestroy()
        {
            this.CapModeChangedEvent -= this.ChangeActiveCAPPanel;
        }

        #endregion Methods
    }
}
