using UnityEngine;

public class TrafficLightBuilder : GridSelector {

	private enum State {
		SELECTING_START, 
		SELECTING_END
	}

	public TrafficLight trafficLightPrefab;
	
	private State state;
	
	private Grid startGrid;
	private Grid endGrid;

	public override void OnActivated() {
		base.OnActivated();

		state = State.SELECTING_START;

		UIManager.Instance.Prompt("Select traffic light start point");
	}

	protected override void Update() {
		base.Update();

		if (!active) {
			return;
		}

		switch (state) {
			case State.SELECTING_START:
				InputSelectStartPoint((Grid) selected);
				break;
			case State.SELECTING_END:
				InputSelectEndPoint((Grid) selected);
				break;
			default:
				state = State.SELECTING_START;
				break;
		}
	}

	private void InputSelectStartPoint(Grid highlightedGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (highlightedGrid != null) {
				startGrid = highlightedGrid;

				state = State.SELECTING_END;

				UIManager.Instance.Prompt("Select traffic light end point");
			}
		}
	}

	private void InputSelectEndPoint(Grid highlightedGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (highlightedGrid != null) {
				endGrid = highlightedGrid;

				Vector3 trafficLightDimensions = new Vector3(
					Mathf.Abs(endGrid.Coordinates.x - startGrid.Coordinates.x) + 1,
					1,
					Mathf.Abs(endGrid.Coordinates.z - startGrid.Coordinates.z) + 1);
				Vector3 trafficLightPosition = (startGrid.transform.position + endGrid.transform.position) / 2.0f;
				TrafficLight trafficLight = Instantiate(trafficLightPrefab, trafficLightPosition, Quaternion.identity);
				trafficLight.transform.localScale = new Vector3(
					trafficLightDimensions.x,
					1.0f,
					trafficLightDimensions.z);

				state = State.SELECTING_START;

				UIManager.Instance.Prompt("Traffic light successfully built");
			}
		}
	}

}
