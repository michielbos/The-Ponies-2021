using UnityEngine;

namespace Assets.Scripts.Controllers {

public class CameraController : VolatileSingletonController<CameraController> {
    public Transform holder;
    public float minSize = 1, maxSize = 32;

    private bool clicked;
    private Vector3 panStartMouse;
    private bool dragging;
    private new Camera camera;

    private void Start() {
        camera = Camera.main;
    }

    public void Update() {
        if (Input.GetButtonDown("Fire3")) {
            clicked = !clicked;
        }
        if (Input.GetButtonDown("Fire2")) {
            panStartMouse = Input.mousePosition;
            dragging = true;
        }
        if (Input.GetButtonUp("Fire2")) {
            dragging = false;
        }

        if (dragging) {
            Vector3 camForward = LevelVector(camera.transform.forward);
            Vector3 camRight = LevelVector(camera.transform.right);

            Vector3 position = holder.position
                              + camForward * (Input.mousePosition - panStartMouse).y * camera.orthographicSize /
                              Screen.height
                              + camRight * (Input.mousePosition - panStartMouse).x * camera.orthographicSize /
                              Screen.width;
            var property = PropertyController.Instance.property;
            position.x = Mathf.Clamp(position.x, 0f, property.TerrainWidth);
            position.z = Mathf.Clamp(position.z, 0f, property.TerrainHeight);
            holder.position = position;
        }

        int scrollDir = (int) Mathf.Clamp(Input.mouseScrollDelta.y, -1, 1);
        Zoom(scrollDir);
    }

    public void Rotate(bool cc) {
        holder.Rotate(0, cc ? -90 : 90, 0);
    }

    public void Zoom(int zoomDir) {
        float zoom = 1;
        if (zoomDir > 0)
            zoom = 0.5f;
        if (zoomDir < 0)
            zoom = 2;

        camera.orthographicSize *= zoom;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minSize, maxSize);
    }

    private Vector3 LevelVector(Vector3 v) {
        return Vector3.ProjectOnPlane(v, Vector3.up).normalized;
    }
}

}