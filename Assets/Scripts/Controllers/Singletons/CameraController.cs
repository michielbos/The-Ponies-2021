using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	public class CameraController : SingletonMonoBehaviour<CameraController>
	{
		public Transform holder;
		public float minSize = 1, maxSize = 32;

		private bool clicked;
		private Vector3 panStartMouse;
		private Vector3 panStartPos;

		public void Rotate(bool cc)
		{
			holder.Rotate(0, cc ? -90 : 90, 0);
		}

		public void Zoom(int zoomDir)
		{
			Camera.main.orthographicSize *= zoomDir < 0 ? .5f : zoomDir > 0 ? 2 : 1;
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minSize, maxSize);
		}

		public void Update()
		{
			if (Input.GetButtonDown("Fire3"))
			{
				clicked = !clicked;
			}
			if (Input.GetButtonDown("Fire2"))
			{
				panStartMouse = Input.mousePosition;
				panStartPos = holder.transform.position;
			}
			if (Input.GetButton("Fire2"))
			{
				holder.position = panStartPos + LevelVector(Camera.main.transform.forward) * (Input.mousePosition - panStartMouse).y * 6 * Camera.main.orthographicSize / Screen.height
				                  + LevelVector(Camera.main.transform.right) * (Input.mousePosition - panStartMouse).x * 4 * Camera.main.orthographicSize / Screen.width;
			}

			int scrollDir = (int)Mathf.Clamp(Input.mouseScrollDelta.y, -1, 1);
			Zoom(scrollDir);
		}

		private Vector3 LevelVector(Vector3 v)
		{
			return Vector3.ProjectOnPlane(v, Vector3.up);
		}
	}
}