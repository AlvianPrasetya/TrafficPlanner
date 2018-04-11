using UnityEngine;

public class InfographicSelector : Selector<Infographic> {

	protected override void Awake() {
		base.Awake();

		active = true;
	}

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.INFOGRAPHIC)) {
			Infographic infographicCandidate = hitInfo.transform.GetComponent<Infographic>();

			if (infographicCandidate != selected) {
				if (selected != null) {
					if (selected.IsActive) {
						selected.ToggleActive();
					}
					selected = null;
				}

				if (infographicCandidate != null) {
					selected = infographicCandidate;
					if (!selected.IsActive) {
						selected.ToggleActive();
					}
				}
			}
		} else {
			if (selected != null) {
				if (selected.IsActive) {
					selected.ToggleActive();
				}
				selected = null;
			}
		}
	}

}
