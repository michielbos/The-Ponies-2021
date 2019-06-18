using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Util {

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T> {
    public static bool HasInstance => instance != null;
    private static T instance;
        
    public static T Instance => GetInstance();
    [CanBeNull] public static T InstanceOrNull => GetInstanceOrNull();

    public static T GetInstance() {
        if (instance == null) {
            Debug.LogWarning("The " + typeof(T) + " instance was requested before it was available!");
        }
        return instance;
    }
    
    [CanBeNull]
    public static T GetInstanceOrNull() {
        return instance;
    }

    public void Awake() {
        if (instance != null) {
            Debug.LogWarning("A new " + typeof(T) + " was instantiated before the previous one was destroyed!");
        }
        instance = (T) this;
    }
}

}