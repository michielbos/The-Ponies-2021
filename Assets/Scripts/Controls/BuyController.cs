using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyController : MonoBehaviour {
	public GameObject buildMarkerPrefab;
	public AudioClip buySound;
	public AudioClip denySound;
	public PropertyController propertyController;
	public AudioSource audioSource;

	private int placingType = -1;
	private GameObject buildMarker;
	private TerrainTile pressedTile = null;

	public void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (placingType >= 0) {
			if (pressedTile == null) {
				if (Physics.Raycast(ray, out hit, 1000, 1 << 8)) {
					buildMarker.transform.position = new Vector3(hit.transform.position.x + 0.5f, 0, hit.transform.position.z + 0.5f);
					if (Input.GetMouseButtonDown(0)) {
						pressedTile = hit.collider.GetComponent<TerrainTile>();
					}
				} else {
					buildMarker.transform.position = new Vector3(0, -100, 0);
				}
			} else {
				if (Input.GetMouseButtonUp(0)) {
					audioSource.PlayOneShot(buySound);
					propertyController.PlacePropertyObject(pressedTile.x, pressedTile.y, ObjectRotation.NORTH, placingType);
					pressedTile = null;
					if (!Input.GetKey(KeyCode.LeftShift)) {
						StopBuying();
					}
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.Delete)) {
			StopBuying();
		}
	}

	private void StopBuying () {
		placingType = -1;
		if (buildMarker != null) {
			Destroy(buildMarker);
			buildMarker = null;
		}
	}

	public void OnEnable () {
		placingType = 1;
		buildMarker = Instantiate(buildMarkerPrefab);
	}

	public void OnDisable () {
		placingType = -1;
		Destroy(buildMarker);
		buildMarker = null;
	}
}
