﻿using UnityEngine;

public class DemolishableRoadSelector : Selector<Road> {

	public override void OnDeactivated() {
		base.OnDeactivated();

		if (selected != null) {
			selected.UnindicateDemolish();
			selected = null;
		}
	}

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);

		if (selected != null) {
			selected.UnindicateDemolish();
			selected = null;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.ROAD)) {
			Road demolishableRoadCandidate = hitInfo.transform.GetComponentInParent<Road>();
			if (demolishableRoadCandidate.IsDemolishable) {
				selected = demolishableRoadCandidate;
				selected.IndicateDemolish();
			}
		}
	}

}
