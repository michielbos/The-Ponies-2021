using UnityEngine;
using UnityEngine.UI;

namespace UI.CAP
{
    /// <summary>
    /// Controls the text description of the races
    /// </summary>
    public class RaceDescriptionController : MonoBehaviour
    {
        /// <summary>
        /// The area used to display the race description
        /// </summary>
        [SerializeField]
        private Text _raceDescriptionArea = null;

        /// <summary>
        /// The earth pony description
        /// </summary>
        [SerializeField]
        [Multiline(5)]
        private string _earthPonyDescription = "";

        /// <summary>
        /// The pegasus description
        /// </summary>
        [SerializeField]
        [Multiline(5)]
        private string _pegasusDescription = "";

        /// <summary>
        /// The unicorn description
        /// </summary>
        [SerializeField]
        [Multiline(5)]
        private string _unicornDescription = "";

        /// <summary>
        /// Sets the description to the given string.
        /// </summary>
        /// <param name="description">The description to show</param>
        private void SetRaceDescription(string description)
        {
            _raceDescriptionArea.text = description;
        }

        /// <summary>
        /// Sets the race description to earth pony 
        /// </summary>
        public void SetEarthPonyDescription()
        {
            SetRaceDescription(_earthPonyDescription);
        }

        /// <summary>
        /// Sets the race description to pegasus
        /// </summary>
        public void SetPegasusDescription()
        {
            SetRaceDescription(_pegasusDescription);
        }

        /// <summary>
        /// Sets the race description to unicorn
        /// </summary>
        public void SetUnicornDescription()
        {
            SetRaceDescription(_unicornDescription);
        }
    }
}
