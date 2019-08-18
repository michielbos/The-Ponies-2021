using UnityEngine;

namespace Util {

/// <summary>
/// A simple component that manages a child model for a GameObject.
/// </summary>
public class ModelContainer : MonoBehaviour {
    public GameObject Model { get; private set; }

    public void InstantiateModel(GameObject prefab) {
        if (Model != null)
            Destroy(Model);
        Model = Instantiate(prefab, transform);
    }
}

}