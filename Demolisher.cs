using UnityEngine;

public class Demolisher : Selector {

	protected override void Update() {
		base.Update();

		if (!active) {
			return;
		}
		
		InputDemolish();
	}

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);
		
		if (selected != null) {
			selected.Unselect();
			selected = null;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.STRUCTURE)) {
			Structure candidateStructure = hitInfo.transform.GetComponentInParent<Structure>();
			if (candidateStructure.IsSelectable) {
				selected = candidateStructure;
				selected.Select();
			}
		}
	}

	private void InputDemolish() {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT) && selected != null) {
			((Structure) selected).Demolish();
			selected = null;
		}
	}

}
