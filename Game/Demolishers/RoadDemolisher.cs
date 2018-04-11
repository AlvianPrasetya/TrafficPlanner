using UnityEngine;

public class RoadDemolisher : DemolishableRoadSelector {

	protected override void Update() {
		base.Update();

		if (!active) {
			return;
		}
		
		InputDemolish();
	}
	
	private void InputDemolish() {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT) && selected != null) {
			selected.Demolish();
			selected = null;
		}
	}

}
