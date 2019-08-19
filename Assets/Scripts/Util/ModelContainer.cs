using UnityEngine;

namespace Util {

/// <summary>
/// A simple component that manages a child model for a GameObject.
/// </summary>
public class ModelContainer : MonoBehaviour {
    public GameObject Model { get; private set; }

    public void InstantiateModel(GameObject prefab) {
        if (Model != null) {
            // Deactivate the model, to hide it before the delayed destruction is completed.
            Model.SetActive(false);
            Destroy(Model);
        }
        Model = Instantiate(prefab, transform);
        Model.SetActive(true);
    }
}

}