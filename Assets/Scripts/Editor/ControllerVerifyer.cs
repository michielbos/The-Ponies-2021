using Assets.Scripts.Controllers;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
	[CustomEditor(typeof(DebugControllerStub))]
	public class ControllerVerifyer : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (Application.isPlaying)
			{
				TestSingleton(CameraController.Instance, "CameraController");
				TestSingleton(CheatsController.Instance, "CheatsController");
				TestSingleton(GameController.Instance, "GameController");
				TestSingleton(MoneyController.Instance, "MoneyController");
				TestSingleton(MusicController.Instance, "MusicController");
				TestSingleton(PropertyController.Instance, "PropertyController");
				TestSingleton(SoundController.Instance, "SoundController");
				TestSingleton(SpeedController.Instance, "SpeedController");
			}
			else
			{
				EditorGUILayout.HelpBox("Please enter play mode to review controller states.", MessageType.Info);
			}
		}

		private void TestSingleton(object instance, string name)
		{
			if (instance == null)
				EditorGUILayout.HelpBox(string.Format("{0} has a null instance", name), MessageType.Error);
			else
				EditorGUILayout.HelpBox(string.Format("{0} is valid", name), MessageType.Info);
		}
	}
}