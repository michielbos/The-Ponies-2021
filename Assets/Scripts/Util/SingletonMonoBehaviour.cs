using UnityEngine;

namespace Assets.Scripts.Util
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<SingletonMonoBehaviour<T>>();
				}
				return (T) _instance;
			}
		}
		private static object _instance;

		// Clear the instance when ending play mode
		void OnApplicationQuit()
		{
			_instance = null;
		}
	}
}