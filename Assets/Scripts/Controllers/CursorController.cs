using Assets.Scripts.Util;
using UnityEngine;

namespace Controllers {

public enum CursorType {
    Normal = 0,
    Unavailable = 1,
    GoHere = 2,
    Interact = 3,
    InteractPony = 4,
    Hourglass = 5,
    Hand = 6,
    Rotate = 7,
    Terrain = 8,
    Delete = 9,
    Bulldozer = 10
}

public class CursorController : SingletonMonoBehaviour<CursorController> {
    public Texture2D[] cursorTextures;
    private CursorType currentCursor;

    public void SetCursor(CursorType cursorType) {
        if (cursorType == currentCursor)
            return;
        currentCursor = cursorType;
        Cursor.SetCursor(cursorTextures[(int) cursorType], Vector2.zero, CursorMode.Auto);
    }
}

}