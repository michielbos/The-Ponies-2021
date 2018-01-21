using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
	[CreateAssetMenu(fileName = "WindowMap", menuName = "The Ponies/Window Map")]
	public class WindowMap : ScriptableObject
	{
		public List<WindowMapEntry> MapEntries { get { return _mapEntries; } }
		[SerializeField]
		private List<WindowMapEntry> _mapEntries = new List<WindowMapEntry>();

		[Serializable]
		public class WindowMapEntry
		{
			[SerializeField]
			private GameObject _windowPrefab;
			[SerializeField]
			private Texture2D _windowCutoutTexture;
			[SerializeField]
			private string _windowId;

			public GameObject Prefab
			{
				get { return _windowPrefab; }
				set { _windowPrefab = value; }
			}
			public Texture2D CutoutTexture
			{
				get { return _windowCutoutTexture; }
				set { _windowCutoutTexture = value; }
			}
			public Guid Id
			{
				get { return new Guid(_windowId); }
				set { _windowId = value.ToString(); }
			}
		}
	}
}