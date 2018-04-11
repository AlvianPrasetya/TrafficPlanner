using UnityEngine;

public class RoadPointSelector : Selector<Vector3> {

	protected bool lastIsValidSelection;
	protected bool isValidSelection;

	protected override void Awake() {
		base.Awake();

		lastIsValidSelection = false;
		isValidSelection = false;
		selected = Vector3.zero;
	}

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.ROAD)) {
			lastIsValidSelection = isValidSelection;
			isValidSelection = true;

			selected = hitInfo.point;
		} else {
			lastIsValidSelection = isValidSelection;
			isValidSelection = false;

			selected = Vector3.zero;
		}
	}

}
