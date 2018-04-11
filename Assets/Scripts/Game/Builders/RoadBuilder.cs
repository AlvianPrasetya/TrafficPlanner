using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

	public EntryRoad BuildEntryRoad(Grid entryGrid) {
		return BuildEntryRoad(entryGrid.Coordinates);
	}

	public EntryRoad BuildEntryRoad(Vector3 entryCoordinates) {
		Grid startGrid, endGrid;
		GetEntryRoadStartEndGrids(entryCoordinates, out startGrid, out endGrid);

		EntryRoad entryRoad = Instantiate(roadFactory.GetEntryRoadPrefab(),
			startGrid.transform.position, startGrid.transform.rotation,
			SiteManager.Instance.roadManager.transform);
		entryRoad.StartGrid = startGrid;
		entryRoad.EndGrid = endGrid;

		SplitIntersectingRoads(entryRoad);

		return entryRoad;
	}

	public ExitRoad BuildExitRoad(Grid exitGrid) {
		return BuildExitRoad(exitGrid.Coordinates);
	}

	public ExitRoad BuildExitRoad(Vector3 exitCoordinates) {
		Grid startGrid, endGrid;
		GetExitRoadStartEndGrids(exitCoordinates, out startGrid, out endGrid);

		ExitRoad exitRoad = Instantiate(roadFactory.GetExitRoadPrefab(),
			startGrid.transform.position, startGrid.transform.rotation,
			SiteManager.Instance.roadManager.transform);
		exitRoad.StartGrid = startGrid;
		exitRoad.EndGrid = endGrid;

		SplitIntersectingRoads(exitRoad);

		return exitRoad;
	}

	public Road BuildArterial(Grid startGrid, Grid endGrid) {
		Road arterial = Instantiate(roadFactory.GetArterialPrefab(),
			startGrid.transform.position, startGrid.transform.rotation,
			SiteManager.Instance.roadManager.transform);
		arterial.StartGrid = startGrid;
		arterial.EndGrid = endGrid;

		SplitIntersectingRoads(arterial);

		return arterial;
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
				InputStartBuilding(selected);
				break;
			case State.BUILDING:
				UpdateRoadPreview(selected);
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
				roadPreview = Instantiate(roadFactory.GetRoadPreviewPrefab(), 
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
		roadPreview.EndGrid = candidateEndGrid;

		if (roadPreview.IsValid) {
			Road candidateRoadPrefab = roadFactory.GetRoadPrefab(roadPreview.StartGrid, roadPreview.EndGrid);
			if (candidateRoadPrefab is TransactableRoad) {
				UIManager.Instance.ShowOnMousePrompt(
					string.Format("$ {0:0.00}", ((TransactableRoad) candidateRoadPrefab).costPerUnit * roadPreview.RoadLength));
			} else {
				UIManager.Instance.HideOnMousePrompt();
			}
		} else {
			UIManager.Instance.ShowOnMousePrompt(roadPreview.InvalidStatus);
		}
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

		UIManager.Instance.HideOnMousePrompt();
	}

	private void SplitIntersectingRoads(Road road) {
		Vector3 roadStart = road.StartGrid.transform.position;
		Vector3 roadEnd = road.EndGrid.transform.position;

		List<GridRoadPair> intersectionInfoList = new List<GridRoadPair>();

		IEnumerable<Road> otherRoads = SiteManager.Instance.roadManager.Roads
			.Concat(SiteManager.Instance.roadManager.Arterials.Cast<Road>())
			.Concat(SiteManager.Instance.trafficManager.trafficBuilder.EntryRoads.Cast<Road>())
			.Concat(SiteManager.Instance.trafficManager.trafficBuilder.ExitRoads.Cast<Road>());

		foreach (Road other in otherRoads) {
			if (other == road) {
				// Do not check for self-intersection
				continue;
			}

			Vector3 otherStart = other.StartGrid.transform.position;
			Vector3 otherEnd = other.EndGrid.transform.position;

			Vector3 intersection;
			if (VectorUtils.AreLineSegmentsIntersecting(roadStart, roadEnd, otherStart, otherEnd, out intersection)) {
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

	public void GetEntryRoadStartEndGrids(Vector3 entryCoordinates, out Grid startGrid, out Grid endGrid) {
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;

		Vector3 startGridCoordinates;
		if (entryCoordinates.x == 1) {
			startGridCoordinates.x = 0;
		} else if (entryCoordinates.x == siteDimensions.x) {
			startGridCoordinates.x = siteDimensions.x + 1;
		} else {
			startGridCoordinates.x = entryCoordinates.x;
		}

		startGridCoordinates.y = entryCoordinates.y;

		if (entryCoordinates.z == 1) {
			startGridCoordinates.z = 0;
		} else if (entryCoordinates.z == siteDimensions.z) {
			startGridCoordinates.z = siteDimensions.z + 1;
		} else {
			startGridCoordinates.z = entryCoordinates.z;
		}

		Vector3 endGridCoordinates = entryCoordinates;

		startGrid = SiteManager.Instance.gridManager.GetGrid(startGridCoordinates);
		endGrid = SiteManager.Instance.gridManager.GetGrid(endGridCoordinates);
		return;
	}

	public void GetExitRoadStartEndGrids(Vector3 exitCoordinates, out Grid startGrid, out Grid endGrid) {
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;

		Vector3 startGridCoordinates = exitCoordinates;

		Vector3 endGridCoordinates;
		if (exitCoordinates.x == 1) {
			endGridCoordinates.x = 0;
		} else if (exitCoordinates.x == siteDimensions.x) {
			endGridCoordinates.x = siteDimensions.x + 1;
		} else {
			endGridCoordinates.x = exitCoordinates.x;
		}

		endGridCoordinates.y = exitCoordinates.y;

		if (exitCoordinates.z == 1) {
			endGridCoordinates.z = 0;
		} else if (exitCoordinates.z == siteDimensions.z) {
			endGridCoordinates.z = siteDimensions.z + 1;
		} else {
			endGridCoordinates.z = exitCoordinates.z;
		}

		startGrid = SiteManager.Instance.gridManager.GetGrid(startGridCoordinates);
		endGrid = SiteManager.Instance.gridManager.GetGrid(endGridCoordinates);
		return;
	}

}
