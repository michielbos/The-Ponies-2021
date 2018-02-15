using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controls
{
	public class BuildTool : Tool
	{
		public GameObject markerPrefab;
		public HUDController hudControlelr;
		public bool offsetMarker;
		[HideInInspector]
		public GameObject buildMarker;
		
		private float positionLockX;
		private float positionLockY;
		private bool positionLockActive;

		void OnEnable()
		{
			positionLockActive = false;
		}

		void OnDisable()
		{
			buildMarker.SetActive(false);
		}

		void Start()
		{
			buildMarker = Instantiate(markerPrefab);
		}

		void Update()
		{
			bool active;
			Vector3 markerPos = GetBuildMarkerPosition(out active);
			buildMarker.SetActive(active);
			if (!active)
				return;

			buildMarker.transform.position = markerPos;
			bool mouseClick = Input.GetMouseButtonDown(0);
			bool abortClick = Input.GetKeyDown(KeyCode.Escape);
			if (mouseClick || abortClick)
			{
				if (positionLockActive)
				{
					FinishWallSegment(markerPos, abortClick);
				}
				else if (mouseClick)
				{
					BeginWallSegment(markerPos);
				}
				positionLockActive = !positionLockActive;
			}
		}

		private void BeginWallSegment(Vector3 markerPos)
		{
			positionLockX = markerPos.x;
			positionLockY = markerPos.z;

			Debug.Log("Begin wall building");
			// TODO
		}

		private void FinishWallSegment(Vector3 markerPos, bool abort = false)
		{

			if (abort)
				Debug.Log("Aborted wall building");
			else
				Debug.Log("Finished wall building");

			// TODO
		}

		private Vector3 GetBuildMarkerPosition(out bool hit)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 1000, 1 << 8 /*Terrain layer*/) && !hudControlelr.IsMouseOverGui())
			{
				hit = true;
				float hitX = raycastHit.point.x;
				float hitY = raycastHit.point.z;
				hitX = Mathf.Floor(hitX + (offsetMarker ? 0.5f : 0));
				hitY = Mathf.Floor(hitY + (offsetMarker ? 0.5f : 0));

				if (positionLockActive)
				{
					float distX = Mathf.Abs(Mathf.Abs(hitX) - Mathf.Abs(positionLockX));
					float distY = Mathf.Abs(Mathf.Abs(hitY) - Mathf.Abs(positionLockY));
					if (distX < distY)
					{
						hitX = positionLockX;
					}
					else
					{
						hitY = positionLockY;
					}
				}

				return new Vector3(hitX, 0, hitY);
			}
			hit = false;
			return Vector3.zero;
		}

		public override void SetSelectedPreset(CatalogItem catalogItem, int skin)
		{
			throw new NotImplementedException();
		}
	}
}
