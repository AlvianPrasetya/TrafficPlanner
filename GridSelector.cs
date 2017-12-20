using UnityEngine;

public class GridSelector : Selector {

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);

		if (selected != null) {
			selected.Unselect();
			selected = null;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.SELECTABLE_GRID)) {
			ISelectable selectedCandidate = hitInfo.transform.GetComponent<Grid>();
			if (selectedCandidate.IsSelectable) {
				selected = selectedCandidate;
				selected.Select();
			}
		}
	}

}
