using UnityEngine;

public class GrassBuilder : MonoBehaviour {

	private enum State {
		SELECTING_START,
		SELECTING_END
	}

	private bool active;
	private State state;

	private Grid startGrid;
	private Grid endGrid;

	public void ToggleActive() {
		if (active) {
			active = false;
		} else {
			active = true;

			UIManager.Instance.Prompt("Select grass start point");
		}
	}

	/*private void Awake() {
		active = false;
		state = State.SELECTING_START;
	}

	private void Update() {
		if (active) {
			switch (state) {
				case State.SELECTING_START:
					InputSelectStartPoint(SiteManager.Instance.gridManager.HighlightedGrid);
					break;
				case State.SELECTING_END:
					InputSelectEndPoint(SiteManager.Instance.gridManager.HighlightedGrid);
					break;
				default:
					state = State.SELECTING_START;
					break;
			}
		}
	}

	private void InputSelectStartPoint(Grid highlightedGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (highlightedGrid != null) {
				startGrid = highlightedGrid;

				state = State.SELECTING_END;

				UIManager.Instance.Prompt("Select grass end point");
			}
		}
	}

	private void InputSelectEndPoint(Grid highlightedGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (highlightedGrid != null) {
				endGrid = highlightedGrid;

				int minX = (int) Mathf.Min(startGrid.Coordinates.x, endGrid.Coordinates.x);
				int maxX = (int) Mathf.Max(startGrid.Coordinates.x, endGrid.Coordinates.x);
				int minZ = (int) Mathf.Min(startGrid.Coordinates.z, endGrid.Coordinates.z);
				int maxZ = (int) Mathf.Max(startGrid.Coordinates.z, endGrid.Coordinates.z);
				for (int x = minX; x <= maxX; x++) {
					for (int z = minZ; z <= maxZ; z++) {
						Grid grid = SiteManager.Instance.gridManager.Grids[x, (int) startGrid.Coordinates.y, z];
						grid.IsGrass = true;
					}
				}

				state = State.SELECTING_START;

				UIManager.Instance.Prompt("Grass successfully grown");
			}
		}
	}*/

}
