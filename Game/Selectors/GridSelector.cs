using UnityEngine;

public class GridSelector : Selector<Grid> {
	
	private Grid selectedBaseGrid;

	public override void OnActivated() {
		base.OnActivated();
		
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;
		for (int x = 1; x <= siteDimensions.x; x++) {
			for (int z = 1; z <= siteDimensions.z; z++) {
				for (int y = 1; y <= siteDimensions.y; y++) {
					Grid grid = SiteManager.Instance.gridManager.GetGrid(x, y, z);
					if (grid.State == Grid.GridState.EMPTY) {
						grid.State = Grid.GridState.BASE;
						break;
					}
				}
			}
		}
	}

	public override void OnDeactivated() {
		base.OnDeactivated();
		
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;
		for (int x = 1; x <= siteDimensions.x; x++) {
			for (int z = 1; z <= siteDimensions.z; z++) {
				for (int y = 1; y <= siteDimensions.y; y++) {
					Grid grid = SiteManager.Instance.gridManager.GetGrid(x, y, z);
					if (grid.State == Grid.GridState.BASE) {
						grid.State = Grid.GridState.EMPTY;
						break;
					}
				}
			}
		}
	}

	protected override void UpdateSelection() {
		Ray cameraRay = referenceCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo;
		if (Physics.Raycast(cameraRay, out hitInfo, Mathf.Infinity, LayerUtils.Mask.BASE_GRID)) {
			Grid selectedBaseCandidate = hitInfo.transform.GetComponent<Grid>();
			if (selectedBaseCandidate != selectedBaseGrid) {
				// Selected base grid has changed since last frame, update the base grid and reset selected
				if (selected != null) {
					selected.Unselect();
				}

				selectedBaseGrid = selectedBaseCandidate;
				selected = selectedBaseGrid;
				selected.Select();
			}

			UpdateSelectedGridVertically();
			return;
		}

		selectedBaseGrid = null;
		if (selected != null) {
			selected.Unselect();
			selected = null;
		}
	}

	private void UpdateSelectedGridVertically() {
		float scrollValue = Input.GetAxis("Mouse ScrollWheel");

		selected.Unselect();

		Grid gridAbove = selected.GetNeighbour(Vector3.up);
		Grid gridBelow = selected.GetNeighbour(Vector3.down);
		if ((scrollValue < 0.0f || Input.GetKeyDown(KeyCode.S)) && gridBelow != null) {
			selected = gridBelow;
		} else if ((scrollValue > 0.0f || Input.GetKeyDown(KeyCode.W)) && gridAbove != null) {
			selected = gridAbove;
		}

		selected.Select();
	}

}
