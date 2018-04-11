using UnityEngine;

public class DemolishableLandmarkSelector : Selector<Landmark> {

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);

		if (selected != null) {
			selected.UnindicateDemolish();
			selected = null;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.LANDMARK)) {
			Landmark demolishableLandmarkCandidate = hitInfo.transform.GetComponentInParent<Landmark>();
			if (demolishableLandmarkCandidate.IsDemolishable) {
				selected = demolishableLandmarkCandidate;
				selected.IndicateDemolish();
			}
		}
	}

}
