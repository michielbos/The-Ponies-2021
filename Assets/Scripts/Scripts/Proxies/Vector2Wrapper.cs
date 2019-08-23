using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

namespace Scripts.Proxies {

[MoonSharpUserData]
public class Vector2Wrapper {
    private Vector2 vector2;

    [MoonSharpVisible(false)]
    public Vector2Wrapper(Vector2 vector2) {
        this.vector2 = vector2;
    }

    [MoonSharpVisible(false)]
    public Vector2Wrapper(float x, float y) {
        vector2 = new Vector2(x, y);
    }

    [MoonSharpVisible(false)]
    public static Vector2Wrapper Create(float x, float y) {
        return new Vector2Wrapper(x, y);
    }

    [MoonSharpVisible(false)]
    public Vector2 Get() {
        return vector2;
    }

    [MoonSharpVisible(false)]
    public Vector2Int GetVector2Int() {
        return new Vector2Int((int) vector2.x, (int) vector2.y);
    }

    public float x => vector2.x;

    public float y => vector2.y;

    public override string ToString() {
        return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
    }
}

}