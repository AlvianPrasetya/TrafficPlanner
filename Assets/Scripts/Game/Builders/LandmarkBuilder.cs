using UnityEngine;

public class LandmarkBuilder : GridSelector {

	private enum State {
		IDLE,
		BUILDING
	}

	public LandmarkFactory landmarkFactory;

	private State state;

	private int landmarkAssetId;

	private LandmarkPreview landmarkPreview;

	public void InitializeBuilder(int landmarkAssetId) {
		this.landmarkAssetId = landmarkAssetId;
	}

	public Landmark BuildLandmark(int landmarkAssetId, Grid startGrid, Grid endGrid) {
		Landmark landmark = Instantiate(
			landmarkFactory.GetLandmarkPrefab(landmarkAssetId),
			startGrid.transform.position, Quaternion.identity, 
			SiteManager.Instance.landmarkManager.transform);
		landmark.StartGrid = startGrid;
		landmark.EndGrid = endGrid;

		return landmark;
	}

	protected override void Awake() {
		base.Awake();

		state = State.IDLE;
	}

	protected override void Update() {
		base.Update();

		if (!active) {
			return;
		}

		switch (state) {
			case State.IDLE:
				InputStartBuilding((Grid) selected);
				break;
			case State.BUILDING:
				UpdateRoadPreview((Grid) selected);
				InputEndBuilding();
				break;
			default:
				break;
		}
	}

	private void InputStartBuilding(Grid startGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (startGrid != null) {
				state = State.BUILDING;
				landmarkPreview = Instantiate(
					landmarkFactory.GetLandmarkPreviewPrefab(landmarkAssetId),
					startGrid.transform.position - new Vector3(0.5f, 0.0f, 0.5f), Quaternion.identity,
					transform);
				landmarkPreview.StartGrid = startGrid;
			} else {
				// Invalid start grid, cancel road building operation
				ResetState();
			}
		}
	}

	private void UpdateRoadPreview(Grid endGrid) {
		landmarkPreview.EndGrid = endGrid;
	}

	private void InputEndBuilding() {
		if (Input.GetMouseButtonUp(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (landmarkPreview.IsValid) {
				BuildLandmark(landmarkAssetId, landmarkPreview.StartGrid, landmarkPreview.EndGrid);
			}

			// End landmark building operation
			ResetState();
		}
	}

	private void ResetState() {
		state = State.IDLE;
		// Remove existing landmark preview on state reset
		if (landmarkPreview != null) {
			Destroy(landmarkPreview.gameObject);
			landmarkPreview = null;
		}
	}

}
