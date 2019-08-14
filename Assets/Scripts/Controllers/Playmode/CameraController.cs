using Assets.Scripts.Util;
using Model.Property;
using UI;
using UnityEngine;

namespace Controllers.Playmode {

public class CameraController : SingletonMonoBehaviour<CameraController> {
    private const float KeyboardMoveSpeed = 4f;

    public Transform holder;
    public RectTransform dragOriginIcon;
    public float minSize = 1, maxSize = 32;

    private bool clicked;
    private Vector3 panStartMouse;
    private bool _dragging;
    private new Camera camera;

    private bool Dragging {
        get { return _dragging; }
        set {
            _dragging = value; 
            dragOriginIcon.gameObject.SetActive(value);
        }
    }

    public CameraRotation CameraRotation => (CameraRotation) (Mathf.FloorToInt((holder.eulerAngles.y + 45f) / 90f % 4f) * 90);

    private void Start() {
        camera = Camera.main;
    }

    public void Update() {
        if (Input.GetButtonDown("Fire3")) {
            clicked = !clicked;
        }

        if (Input.GetButtonDown("Fire2")) {
            panStartMouse = Input.mousePosition;
            dragOriginIcon.anchoredPosition = UiScaler.ScalePointToCanvas(panStartMouse);
            Dragging = true;
        }

        if (Input.GetButtonUp("Fire2")) {
            Dragging = false;
        }

        Vector3 camForward = LevelVector(camera.transform.forward);
        Vector3 camRight = LevelVector(camera.transform.right);
        float orthographicSize = camera.orthographicSize;
        Vector3 cameraPosition = holder.position;

        // Drag movement
        if (Dragging) {
            cameraPosition += camForward * (Input.mousePosition - panStartMouse).y * orthographicSize / Screen.height +
                              camRight * (Input.mousePosition - panStartMouse).x * orthographicSize / Screen.width;
        }

        // Keyboard movement
        Vector3 keyboardMovement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (Mathf.Abs(keyboardMovement.x) > 0.1f && Mathf.Abs(keyboardMovement.y) > 0.1f) {
            // sqrt(1² + 1²) = 1.4142
            keyboardMovement /= 1.4142f;
        }

        keyboardMovement *= Time.unscaledDeltaTime * KeyboardMoveSpeed;

        cameraPosition += keyboardMovement.y * orthographicSize * camForward +
                          keyboardMovement.x * orthographicSize * camRight;

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
        WallVisibilityController.Instance.UpdateWallVisibility();
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

public enum CameraRotation {
    North = 0,
    East = 90,
    South = 180,
    West = 270
}

}