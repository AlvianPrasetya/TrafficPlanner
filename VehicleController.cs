using UnityEngine;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour {

	// The maximum distance in which a grid is considered reached
	public float gridReachedDistance;
	public float brakingDistance;
	public float maxSpeed;

	private Grid entryGrid;
	private Grid exitGrid;

	private Grid prevGrid;
	private Grid nextGrid;
	private Grid nextNextGrid;

	public bool halted;

	private new Rigidbody rigidbody;
	private new Collider collider;

	public Grid PrevGrid {
		get {
			return prevGrid;
		}
	}

	public Grid NextGrid {
		get {
			return nextGrid;
		}
	}

	public Grid NextNextGrid {
		get {
			return nextNextGrid;
		}
	}

	public bool IsHalted {
		get {
			return halted;
		}

		set {
			halted = value;
		}
	}

	public void Initialize(Grid entryGrid, Grid exitGrid) {
		this.entryGrid = entryGrid;
		this.exitGrid = exitGrid;
		nextNextGrid = entryGrid;

		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
		IsHalted = false;

		SiteManager.Instance.OnVehicleSpawnedCallback();
	}

	/*private void Update() {
		if (nextGrid == null
			|| Vector3.SqrMagnitude(nextGrid.transform.position - transform.position) < gridReachedDistance * gridReachedDistance) {
			if (nextGrid == exitGrid) {
				SiteManager.Instance.OnVehicleReachedExitCallback();
				Destroy(gameObject);
			}

			UpdatePrevGrid();
			UpdateNextGrid();
			UpdateNextNextGrid();
		}
	}

	private void FixedUpdate() {
		if (prevGrid == null) {
			return;
		}

		if (nextGrid == null) {
			return;
		}

		if (halted) {
			return;
		}

		RaycastHit hitInfo;
		if (!Physics.Raycast(transform.position, transform.forward, out hitInfo, brakingDistance, LayerUtils.Mask.VEHICLE)) {
			transform.Translate((nextGrid.Coordinates - prevGrid.Coordinates) * maxSpeed * Time.fixedDeltaTime, Space.World);
		}

		transform.LookAt(transform.position + (nextGrid.Coordinates - prevGrid.Coordinates));
	}

	private void UpdatePrevGrid() {
		prevGrid = nextGrid;
	}

	private void UpdateNextGrid() {
		nextGrid = nextNextGrid;
		// Disable collider when reaching exit point to avoid exit point queue deadlock
		if (nextGrid != null && nextGrid.IsExitPoint) {
			collider.enabled = false;
		}
	}

	private void UpdateNextNextGrid() {
		if (nextNextGrid == null) {
			return;
		}

		nextNextGrid = DetermineNextGrid(nextNextGrid);
	}

	private Grid DetermineNextGrid(Grid referenceGrid) {
		List<GridDistance> nextGridCandidates = new List<GridDistance>();
		int totalDistanceToExitGrid = 0;

		if (referenceGrid.RoadHorizontalDirection == -1 || referenceGrid.IsEntryPoint) {
			Grid leftGrid = referenceGrid.GetNeighbour(Vector3.left);
			if (leftGrid != null) {
				if (leftGrid.RoadHorizontalDirection != 0 || leftGrid.RoadVerticalDirection != 0 || leftGrid.IsExitPoint) {
					int distance = SiteManager.Instance.gridManager.ShortestPath(leftGrid, exitGrid);
					if (distance != -1) {
						nextGridCandidates.Add(new GridDistance(leftGrid, distance));
						totalDistanceToExitGrid += distance;
					}
				}
			}
		}

		if (referenceGrid.RoadHorizontalDirection == 1 || referenceGrid.IsEntryPoint) {
			Grid rightGrid = referenceGrid.GetNeighbour(Vector3.right);
			if (rightGrid != null) {
				if (rightGrid.RoadHorizontalDirection != 0 || rightGrid.RoadVerticalDirection != 0 || rightGrid.IsExitPoint) {
					int distance = SiteManager.Instance.gridManager.ShortestPath(rightGrid, exitGrid);
					if (distance != -1) {
						nextGridCandidates.Add(new GridDistance(rightGrid, distance));
						totalDistanceToExitGrid += distance;
					}
				}
			}
		}

		if (referenceGrid.RoadVerticalDirection == -1 || referenceGrid.IsEntryPoint) {
			Grid backGrid = referenceGrid.GetNeighbour(Vector3.back);
			if (backGrid != null) {
				if (backGrid.RoadHorizontalDirection != 0 || backGrid.RoadVerticalDirection != 0 || backGrid.IsExitPoint) {
					int distance = SiteManager.Instance.gridManager.ShortestPath(backGrid, exitGrid);
					if (distance != -1) {
						nextGridCandidates.Add(new GridDistance(backGrid, distance));
						totalDistanceToExitGrid += distance;
					}
				}
			}
		}

		if (referenceGrid.RoadVerticalDirection == 1 || referenceGrid.IsEntryPoint) {
			Grid forwardGrid = referenceGrid.GetNeighbour(Vector3.forward);
			if (forwardGrid != null) {
				if (forwardGrid.RoadHorizontalDirection != 0 || forwardGrid.RoadVerticalDirection != 0 || forwardGrid.IsExitPoint) {
					int distance = SiteManager.Instance.gridManager.ShortestPath(forwardGrid, exitGrid);
					if (distance != -1) {
						nextGridCandidates.Add(new GridDistance(forwardGrid, distance));
						totalDistanceToExitGrid += distance;
					}
				}
			}
		}

		if (nextGridCandidates.Count == 1) {
			return nextGridCandidates[0].grid;
		}

		// Sort in ascending order of distance
		nextGridCandidates.Sort(delegate (GridDistance x, GridDistance y) {
			return x.distance.CompareTo(y.distance);
		});

		float[] cumulativeGridOdds = new float[nextGridCandidates.Count];
		for (int i = 0; i < nextGridCandidates.Count; i++) {
			cumulativeGridOdds[i] = ((i == 0) ? 0.0f : cumulativeGridOdds[i - 1])
				+ (float) (nextGridCandidates[nextGridCandidates.Count - 1 - i].distance) / totalDistanceToExitGrid;
		}

		float randomizedOdd = Random.value;
		for (int i = 0; i < nextGridCandidates.Count; i++) {
			if (cumulativeGridOdds[i] > randomizedOdd) {
				return nextGridCandidates[i].grid;
			}
		}

		return null;
	}

	private bool IsNextNextGridCollinear() {
		Vector3 currentDirection = (nextGrid.Coordinates - prevGrid.Coordinates).normalized;
		Vector3 nextDirection = (nextNextGrid.Coordinates - nextGrid.Coordinates).normalized;

		return (currentDirection == nextDirection);
	}

	private void OnCollisionEnter(Collision collision) {
		Destroy(gameObject);
	}*/

}
