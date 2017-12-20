using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	private struct GridPair {

		public Grid first;
		public Grid second;

		public GridPair(Grid first, Grid second) {
			this.first = first;
			this.second = second;
		}

	}

	public GridBuilder gridBuilder;

	private Vector3 siteDimensions;
	private Grid[,,] grids;
	private Dictionary<GridPair, int> shortestPaths;

	public Vector3 SiteDimensions {
		get {
			return siteDimensions;
		}
	}

	public Grid[,,] Grids {
		get {
			return grids;
		}
	}

	public int ShortestPath(Grid source, Grid destination) {
		int shortestPath;
		if (shortestPaths.TryGetValue(new GridPair(source, destination), out shortestPath)) {
			return shortestPath;
		} else {
			return -1;
		}
	}
	
	/*
	 * This method generates building grids according to the supplied dimensions.
	 */
	public void GenerateGrids(SiteDimensionsMetadata siteDimensionsMetadata) {
		siteDimensions = siteDimensionsMetadata.SiteDimensions;
		grids = new Grid[(int) siteDimensions.x + 2, (int) siteDimensions.y + 2, (int) siteDimensions.z + 2];
		
		for (int y = 0; y < siteDimensions.y + 2; y++) {
			for (int x = 0; x < siteDimensions.x + 2; x++) {
				for (int z = 0; z < siteDimensions.z + 2; z++) {
					gridBuilder.BuildGrid(x, y, z);
				}
			}
		}

		LinkGrids();
	}

	public void LinkGrids() {
		for (int y = 0; y < siteDimensions.y + 2; y++) {
			for (int x = 0; x < siteDimensions.x + 2; x++) {
				for (int z = 0; z < siteDimensions.z + 2; z++) {
					if (x != 0) {
						grids[x, y, z].SetNeighbour(Vector3.left, grids[x - 1, y, z]);
						grids[x - 1, y, z].SetNeighbour(Vector3.right, grids[x, y, z]);
					}

					if (y != 0) {
						grids[x, y, z].SetNeighbour(Vector3.down, grids[x, y - 1, z]);
						grids[x, y - 1, z].SetNeighbour(Vector3.up, grids[x, y, z]);
					}

					if (z != 0) {
						grids[x, y, z].SetNeighbour(Vector3.back, grids[x, y, z - 1]);
						grids[x, y, z - 1].SetNeighbour(Vector3.forward, grids[x, y, z]);
					}
				}
			}
		}
	}

	/*public void ParseTrafficFlowsMetadata(string trafficFlowsMetadata) {
		string[] trafficFlowTokens = trafficFlowsMetadata.Split(TRAFFIC_FLOWS_SEPARATOR);

		foreach (string trafficFlowToken in trafficFlowTokens) {
			string[] trafficFlowInfoTokens = trafficFlowToken.Split(TRAFFIC_FLOW_INFO_SEPARATOR);

			string[] entryPointTokens = trafficFlowInfoTokens[0].Split(TRAFFIC_COORDINATES_SEPARATOR);
			string[] exitPointTokens = trafficFlowInfoTokens[1].Split(TRAFFIC_COORDINATES_SEPARATOR);
			// The rate of traffic in # of cars per minute
			int trafficRate = int.Parse(trafficFlowInfoTokens[2]);

			Vector3 entryCoordinate = new Vector3(
					int.Parse(entryPointTokens[0]),
					int.Parse(entryPointTokens[1]),
					int.Parse(entryPointTokens[2]));

			Vector3 exitCoordinate = new Vector3(
				int.Parse(exitPointTokens[0]),
				int.Parse(exitPointTokens[1]),
				int.Parse(exitPointTokens[2]));

			grids[(int) entryCoordinate.x, (int) entryCoordinate.y, (int) entryCoordinate.z].IsEntryPoint = true;
			grids[(int) exitCoordinate.x, (int) exitCoordinate.y, (int) exitCoordinate.z].IsExitPoint = true;
		}
	}*/

	/*
	 * This method finds the closest grid to a given point in 3D space.
	 */
	public Grid GetClosestGrid(Vector3 point) {
		Collider[] hitColliders = Physics.OverlapSphere(
			point, 
			gridBuilder.gridFactory.innerGridPrefab.transform.localScale.x, 
			LayerUtils.Mask.EMPTY_GRID | LayerUtils.Mask.SELECTABLE_GRID);

		Grid closestGrid = null;
		float minSqrDistance = Mathf.Infinity;
		foreach (Collider hitCollider in hitColliders) {
			float sqrDistance = Vector3.SqrMagnitude(point - hitCollider.transform.position);
			if (sqrDistance < minSqrDistance) {
				closestGrid = hitCollider.GetComponent<Grid>();
				minSqrDistance = sqrDistance;
			}
		}

		return closestGrid;
	}

	/*
	 * This method evaluates the shortest path distance of each pair of grids and stores the result 
	 * in a dictionary of grid pairs and their respective shortest path distance.
	 */
	/*public void RecalculateShortestPaths() {
		shortestPaths = new Dictionary<GridPair, int>();

		foreach (Grid grid in grids) {
			if (grid.RoadHorizontalDirection != 0 || grid.RoadVerticalDirection != 0 || grid.IsEntryPoint || grid.IsExitPoint) {
				// Only BFS from roads
				BfsShortestPaths(grid);
			}
		}
	}*/

	private void Update() {
		InputHideSelectableGrids();
		InputShowSelectableGrids();
	}

	/*private void BfsShortestPaths(Grid startingGrid) {
		Queue<GridDistance> outmostGrids = new Queue<GridDistance>();
		Dictionary<Grid, bool> visited = new Dictionary<Grid, bool>();

		// Dummy output redirection variable for visited.TryGetValue()
		bool gridVisited;

		outmostGrids.Enqueue(new GridDistance(startingGrid, 0));
		while (outmostGrids.Count != 0) {
			GridDistance gridDistance = outmostGrids.Dequeue();
			Grid grid = gridDistance.grid;
			int distance = gridDistance.distance;

			// Mark as visited
			visited[grid] = true;

			shortestPaths[new GridPair(startingGrid, grid)] = distance;
			shortestPaths[new GridPair(grid, startingGrid)] = distance;
			// Debug.Log(string.Format("Distance from {0} to {1} is {2}", startingGrid.Coordinates, grid.Coordinates, distance));

			if (grid.RoadHorizontalDirection == -1 || grid.IsEntryPoint) {
				Grid leftGrid = grid.GetNeighbour(Vector3.left);
				if (leftGrid != null && !visited.TryGetValue(leftGrid, out gridVisited)) {
					if (leftGrid.RoadHorizontalDirection != 0 || leftGrid.RoadVerticalDirection != 0 || leftGrid.IsExitPoint) {
						outmostGrids.Enqueue(new GridDistance(leftGrid, distance + 1));
					}
				}
			}

			if (grid.RoadHorizontalDirection == 1 || grid.IsEntryPoint) {
				Grid rightGrid = grid.GetNeighbour(Vector3.right);
				if (rightGrid != null && !visited.TryGetValue(rightGrid, out gridVisited)) {
					if (rightGrid.RoadHorizontalDirection != 0 || rightGrid.RoadVerticalDirection != 0 || rightGrid.IsExitPoint) {
						outmostGrids.Enqueue(new GridDistance(rightGrid, distance + 1));
					}
				}
			}

			if (grid.RoadVerticalDirection == -1 || grid.IsEntryPoint) {
				Grid backGrid = grid.GetNeighbour(Vector3.back);
				if (backGrid != null && !visited.TryGetValue(backGrid, out gridVisited)) {
					if (backGrid.RoadHorizontalDirection != 0 || backGrid.RoadVerticalDirection != 0 || backGrid.IsExitPoint) {
						outmostGrids.Enqueue(new GridDistance(backGrid, distance + 1));
					}
				}
			}

			if (grid.RoadVerticalDirection == 1 || grid.IsEntryPoint) {
				Grid forwardGrid = grid.GetNeighbour(Vector3.forward);
				if (forwardGrid != null && !visited.TryGetValue(forwardGrid, out gridVisited)) {
					if (forwardGrid.RoadHorizontalDirection != 0 || forwardGrid.RoadVerticalDirection != 0 || forwardGrid.IsExitPoint) {
						outmostGrids.Enqueue(new GridDistance(forwardGrid, distance + 1));
					}
				}
			}
		}
	}*/

	private void InputHideSelectableGrids() {
		if (Input.GetKeyDown(KeyCode.LeftAlt)) {
			foreach (Grid grid in grids) {
				if (grid.State == Grid.GridState.SELECTABLE || grid.State == Grid.GridState.SELECTED) {
					grid.IsHidden = true;
				}
			}
		}
	}

	private void InputShowSelectableGrids() {
		if (Input.GetKeyUp(KeyCode.LeftAlt)) {
			foreach (Grid grid in grids) {
				if (grid.State == Grid.GridState.SELECTABLE || grid.State == Grid.GridState.SELECTED) {
					grid.IsHidden = false;
				}
			}
		}
	}

}
