using Assets.Scripts.Util;
using Model.Property;
using UnityEngine;

namespace Controllers.Playmode {

public class CameraController : SingletonMonoBehaviour<CameraController> {
    private const float KeyboardMoveSpeed = 4f;

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

        Vector3 camForward = LevelVector(camera.transform.forward);
        Vector3 camRight = LevelVector(camera.transform.right);
        float orthographicSize = camera.orthographicSize;
        Vector3 cameraPosition = holder.position;

        // Drag movement
        if (dragging) {
            cameraPosition += camForward * (Input.mousePosition - panStartMouse).y * orthographicSize / Screen.height +
                              camRight * (Input.mousePosition - panStartMouse).x * orthographicSize / Screen.width;
        }

        // Keyboard movement
        Vector3 keyboardMovement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ) * Time.deltaTime * KeyboardMoveSpeed;
        cameraPosition += camForward * keyboardMovement.y * orthographicSize +
                          camRight * keyboardMovement.x * orthographicSize;
        

        // Put camera within bounds
        Property property = PropertyController.Instance.property;
        holder.position = new Vector3(
            Mathf.Clamp(cameraPosition.x, 0f, property.TerrainWidth),
            cameraPosition.y,
            Mathf.Clamp(cameraPosition.z, 0f, property.TerrainHeight)
        );

        // Scrolling
        int scrollDir = (int) Mathf.Clamp(Input.mouseScrollDelta.y, -1, 1);
        Zoom(scrollDir);
    }

    public void Rotate(bool counterClockwise) {
        holder.Rotate(0, counterClockwise ? -90 : 90, 0);
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