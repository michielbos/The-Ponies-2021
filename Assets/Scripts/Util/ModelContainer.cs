using UnityEngine;
using UnityGLTF;

namespace Util {

/// <summary>
/// A simple component that manages a child model for a GameObject.
/// </summary>
public class ModelContainer : MonoBehaviour {
    public GameObject Model { get; set; }

    public void InstantiateModel(InstantiatedGLTFObject prefab) {
        if (Model != null) {
            // Deactivate the model, to hide it before the delayed destruction is completed.
            Model.SetActive(false);
            Destroy(Model);
        }
        Model = prefab.Duplicate().gameObject;
        Model.transform.SetParent(gameObject.transform, false);
        Model.SetActive(true);
        SetChildLayers(gameObject.layer);
    }

    private void SetChildLayers(int layer) {
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>()) {
            child.gameObject.layer = layer;
        }
    }
}

}