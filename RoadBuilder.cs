using UnityEngine;
using System.Collections.Generic;

public class RoadBuilder : GridSelector {

	private struct GridRoadPair {

		public Grid grid;
		public Road road;

		public GridRoadPair(Grid grid, Road road) {
			this.grid = grid;
			this.road = road;
		}

	}

	private enum State {
		IDLE, 
		BUILDING
	}

	public RoadFactory roadFactory;

	private State state;
	
	private RoadPreview roadPreview;

	public override void OnActivated() {
		base.OnActivated();

		Grid[,,] grids = SiteManager.Instance.gridManager.Grids;
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;
		for (int x = 1; x <= siteDimensions.x; x++) {
			for (int z = 1; z <= siteDimensions.z; z++) {
				for (int y = 1; y <= siteDimensions.y; y++) {
					if (grids[x, y, z].State == Grid.GridState.EMPTY) {
						grids[x, y, z].State = Grid.GridState.SELECTABLE;
						break;
					}
				}
			}
		}
	}

	public override void OnDeactivated() {
		base.OnDeactivated();

		Grid[,,] grids = SiteManager.Instance.gridManager.Grids;
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;
		for (int x = 1; x <= siteDimensions.x; x++) {
			for (int z = 1; z <= siteDimensions.z; z++) {
				for (int y = 1; y <= siteDimensions.y; y++) {
					if (grids[x, y, z].State == Grid.GridState.SELECTABLE) {
						grids[x, y, z].State = Grid.GridState.EMPTY;
						break;
					}
				}
			}
		}
	}
	
	public Road BuildRoad(Grid startGrid, Grid endGrid) {
		Road road = Instantiate(roadFactory.GetRoadPrefab(startGrid, endGrid),
			startGrid.transform.position, startGrid.transform.rotation,
			SiteManager.Instance.roadManager.transform);
		road.StartGrid = startGrid;
		road.EndGrid = endGrid;

		SplitIntersectingRoads(road);

		return road;
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
				roadPreview = Instantiate(roadFactory.GetRoadPreviewPrefab(startGrid, null), 
					startGrid.transform.position, startGrid.transform.rotation, 
					transform);
				roadPreview.StartGrid = startGrid;
			} else {
				// Invalid start grid, cancel road building operation
				ResetState();
			}
		}
	}

	private void UpdateRoadPreview(Grid candidateEndGrid) {
		RoadPreview oldRoadPreview = roadPreview;

		roadPreview = Instantiate(roadFactory.GetRoadPreviewPrefab(oldRoadPreview.StartGrid, candidateEndGrid),
					oldRoadPreview.StartGrid.transform.position, oldRoadPreview.StartGrid.transform.rotation,
					transform);
		roadPreview.StartGrid = oldRoadPreview.StartGrid;
		roadPreview.EndGrid = candidateEndGrid;

		Destroy(oldRoadPreview.gameObject);
	}

	private void InputEndBuilding() {
		if (Input.GetMouseButtonUp(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (roadPreview.IsValid) {
				BuildRoad(roadPreview.StartGrid, roadPreview.EndGrid);
			}

			// End road building operation
			ResetState();
		}
	}

	private void ResetState() {
		state = State.IDLE;
		// Remove existing road preview on state reset
		if (roadPreview != null) {
			Destroy(roadPreview.gameObject);
			roadPreview = null;
		}
	}

	private void SplitIntersectingRoads(Road road) {
		Vector3 roadStart = road.StartGrid.transform.position;
		Vector3 roadEnd = road.EndGrid.transform.position;

		HashSet<Road> roadsCopy = new HashSet<Road>(SiteManager.Instance.roadManager.Roads);
		List<GridRoadPair> intersectionInfoList = new List<GridRoadPair>();
		foreach (Road other in roadsCopy) {
			if (other == road) {
				// Do not check for self-intersection
				continue;
			}

			Vector3 otherStart = other.StartGrid.transform.position;
			Vector3 otherEnd = other.EndGrid.transform.position;

			Vector3 intersection;
			if (VectorUtils.AreLinesIntersecting(roadStart, roadEnd, otherStart, otherEnd, out intersection)) {
				intersectionInfoList.Add(new GridRoadPair(
					SiteManager.Instance.gridManager.GetClosestGrid(intersection), 
					other));
			}
		}

		intersectionInfoList.Sort((x, y) => Vector3.SqrMagnitude(x.grid.Coordinates - road.StartGrid.Coordinates)
			.CompareTo(Vector3.SqrMagnitude(y.grid.Coordinates - road.StartGrid.Coordinates)));

		Road thisRoad = road;
		foreach (GridRoadPair intersectionInfo in intersectionInfoList) {
			Grid intersectionGrid = intersectionInfo.grid;
			Road otherRoad = intersectionInfo.road;

			IBigridTransform otherRoadHeadBT, otherRoadTailBT;

			IBigridTransform thisRoadHeadBT, thisRoadTailBT;

			otherRoad.Split(intersectionGrid, out otherRoadHeadBT, out otherRoadTailBT);
			thisRoad.Split(intersectionGrid, out thisRoadHeadBT, out thisRoadTailBT);

			Road otherRoadHead = (Road) otherRoadHeadBT;
			Road otherRoadTail = (Road) otherRoadTailBT;
			Road thisRoadHead = (Road) thisRoadHeadBT;
			Road thisRoadTail = (Road) thisRoadTailBT;
			
			/*if (thisRoadHead != null && thisRoadTail != null) {
				thisRoadHead.AddOutgoingRoad(thisRoadTail);
				thisRoadTail.AddIncomingRoad(thisRoadHead);
			}*/

			// Establish links between this road's new head and other road's new tail
			if (thisRoadHead != null && otherRoadTail != null) {
				thisRoadHead.AddOutgoingRoad(otherRoadTail);
				otherRoadTail.AddIncomingRoad(thisRoadHead);
			}

			/*if (otherRoadHead != null && otherRoadTail != null) {
				otherRoadHead.AddOutgoingRoad(otherRoadTail);
				otherRoadTail.AddIncomingRoad(otherRoadHead);
			}*/

			// Establish links between this road's new tail and other road's new head
			if (otherRoadHead != null && thisRoadTail != null) {
				otherRoadHead.AddOutgoingRoad(thisRoadTail);
				thisRoadTail.AddIncomingRoad(otherRoadHead);
			}

			if (thisRoadTail != null) {
				thisRoad = thisRoadTail;
			}
		}
	}

	/*private enum State {
		SELECTING_START, 
		SELECTING_END, 
		ADJUSTING_WIDTH
	}

	// Determines the delta mouse position required to increase road width by 1
	public float deltaMousePositionThreshold;

	private bool active;
	private State state;
	
	private Grid startGrid;
	private Grid endGrid;
	private Vector3 roadDirection;

	// To store initial mouse position (in screen coordinates) for reference during width adjustment step
	private Vector3 initialMousePosition;

	public void ToggleActive() {
		if (active) {
			active = false;
		} else {
			active = true;
			state = State.SELECTING_START;

			UIManager.Instance.Prompt("Select road start point");
		}
	}

	private void Awake() {
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
				case State.ADJUSTING_WIDTH:
					InputAdjustWidth(SiteManager.Instance.gridManager.HighlightedGrid);
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

				UIManager.Instance.Prompt("Select road end point");
			}
		}
	}

	private void InputSelectEndPoint(Grid highlightedGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (highlightedGrid != null) {
				endGrid = highlightedGrid;
				roadDirection = (endGrid.Coordinates - startGrid.Coordinates).normalized;

				// Check if road is either x-aligned or z-aligned
				if (((int) Mathf.Abs(roadDirection.x) ^ (int) Mathf.Abs(roadDirection.z)) == 1) {
					state = State.ADJUSTING_WIDTH;

					UIManager.Instance.Prompt("Adjust road width by moving your mouse to nearby grids, left click to confirm");
				} else {
					// Error, start and end grids are not on a straight line (neither x or z-aligned)
					state = State.SELECTING_START;

					UIManager.Instance.Prompt("Failed to build road. Start and end points must be on a straight line");
				}
			}
		}
	}

	private void InputAdjustWidth(Grid highlightedGrid) {
		// On confirmation of road width, pave the road
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (highlightedGrid != null) {
				if (roadDirection == Vector3.left) {
					// Leftward road
					int roadWidth = (int) highlightedGrid.Coordinates.z - (int) startGrid.Coordinates.z;
					roadWidth += (int) Mathf.Sign(roadWidth);

					for (int x = (int) startGrid.Coordinates.x; x >= (int) endGrid.Coordinates.x; x--) {
						for (int deltaZ = 0; deltaZ != roadWidth; deltaZ += (int) Mathf.Sign(roadWidth)) {
							SiteManager.Instance.gridManager.Grids[
								x,
								(int) startGrid.Coordinates.y,
								(int) startGrid.Coordinates.z + deltaZ]
								.RoadHorizontalDirection = (int) roadDirection.x;
						}
					}
					
					UIManager.Instance.Prompt("Road successfully built");
				} else if (roadDirection == Vector3.right) {
					// Rightward road
					int roadWidth = (int) highlightedGrid.Coordinates.z - (int) startGrid.Coordinates.z;
					roadWidth += (int) Mathf.Sign(roadWidth);

					for (int x = (int) startGrid.Coordinates.x; x <= (int) endGrid.Coordinates.x; x++) {
						for (int deltaZ = 0; deltaZ != roadWidth; deltaZ += (int) Mathf.Sign(roadWidth)) {
							SiteManager.Instance.gridManager.Grids[
								x,
								(int) startGrid.Coordinates.y,
								(int) startGrid.Coordinates.z + deltaZ]
								.RoadHorizontalDirection = (int) roadDirection.x;
						}
					}
					
					UIManager.Instance.Prompt("Road successfully built");
				} else if (roadDirection == Vector3.forward) {
					// Forward road
					int roadWidth = (int) highlightedGrid.Coordinates.x - (int) startGrid.Coordinates.x;
					roadWidth += (int) Mathf.Sign(roadWidth);

					for (int z = (int) startGrid.Coordinates.z; z <= (int) endGrid.Coordinates.z; z++) {
						for (int deltaX = 0; deltaX != roadWidth; deltaX += (int) Mathf.Sign(roadWidth)) {
							SiteManager.Instance.gridManager.Grids[
								(int) startGrid.Coordinates.x + deltaX,
								(int) startGrid.Coordinates.y,
								z]
								.RoadVerticalDirection = (int) roadDirection.z;
						}
					}
					
					UIManager.Instance.Prompt("Road successfully built");
				} else if (roadDirection == Vector3.back) {
					// Backward road
					int roadWidth = (int) highlightedGrid.Coordinates.x - (int) startGrid.Coordinates.x;
					roadWidth += (int) Mathf.Sign(roadWidth);

					for (int z = (int) startGrid.Coordinates.z; z >= (int) endGrid.Coordinates.z; z--) {
						for (int deltaX = 0; deltaX != roadWidth; deltaX += (int) Mathf.Sign(roadWidth)) {
							SiteManager.Instance.gridManager.Grids[
								(int) startGrid.Coordinates.x + deltaX,
								(int) startGrid.Coordinates.y,
								z]
								.RoadVerticalDirection = (int) roadDirection.z;
						}
					}
					
					UIManager.Instance.Prompt("Road successfully built");
				} else {
					// Unexpected state, restore to SELECTING_START state without throwing any error
				}
			}

			state = State.SELECTING_START;
		}
	}*/

}
