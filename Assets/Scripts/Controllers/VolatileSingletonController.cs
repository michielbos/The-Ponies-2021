using UnityEngine;

namespace Assets.Scripts.Controllers {
    public class VolatileSingletonController<T> : MonoBehaviour where T : VolatileSingletonController<T> {
        private static T instance;
        
        public static T Instance => GetInstance();

        public static T GetInstance() {
            if (instance == null) {
                Debug.LogWarning("The " + typeof(T) + " instance was requested before it was available!");
            }
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