using System.Collections.Generic;
using Model.Ponies;
using UnityEngine;

namespace UI {

public class PlayableCharactersView : MonoBehaviour {
    public GameObject[] portraits;

    private void Start() {
        UpdateHousehold();
    }

    public void UpdateHousehold() {
        List<Pony> ponies = PropertyController.Instance.property.household.ponies;
        foreach (GameObject portrait in portraits) {
            portrait.SetActive(false);
        }
        for (int i = 0; i < ponies.Count; i++) {
            // TODO: Insert this pony into the portrait.
            portraits[i].SetActive(true);
        }
    }
}

}