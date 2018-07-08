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
		//private Vector3 panStartPos;
	    private bool draging;

		public void Rotate(bool cc)
		{
			holder.Rotate(0, cc ? -90 : 90, 0);
		}

		public void Zoom(int zoomDir)
		{
		    float zoom = 1;
		    if (zoomDir > 0) zoom = 0.5f;
		    if (zoomDir < 0) zoom = 2;

			Camera.main.orthographicSize *= zoom;
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
				//panStartPos = holder.transform.position;
			    draging = true;
			}
			if (Input.GetButtonUp("Fire2"))
			{
			    draging = false;
			}

            if (draging)
            {

                Vector3 camForward = LevelVector(Camera.main.transform.forward);
                Vector3 camRight = LevelVector(Camera.main.transform.right);

                holder.position = holder.position 
                    + camForward * (Input.mousePosition - panStartMouse).y * Camera.main.orthographicSize / Screen.height
                    + camRight * (Input.mousePosition - panStartMouse).x * Camera.main.orthographicSize / Screen.width;
            }

            int scrollDir = (int)Mathf.Clamp(Input.mouseScrollDelta.y, -1, 1);
			Zoom(scrollDir);
		}

		private Vector3 LevelVector(Vector3 v)
		{
			return Vector3.ProjectOnPlane(v, Vector3.up).normalized;
		}
	}
}