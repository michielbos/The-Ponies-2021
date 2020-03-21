using UnityEngine;

namespace Model.Property {

public class WallCorner : MonoBehaviour {
    public float normalHeight = 2.5f;
    public float loweredHeight = 0.3f;
    public Transform model;
    
    public void SetLowered(bool lowered) {
        Vector3 scale = model.localScale;
        Vector3 position = model.position;
        float height = lowered ? loweredHeight : normalHeight;
        model.localScale = new Vector3(scale.x, height, scale.z);
        model.position = new Vector3(position.x, height / 2f, position.z);
    }
}

}