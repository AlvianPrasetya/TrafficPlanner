using UnityEngine;
using System.Collections;
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

	public Grid GetGrid(Vector3 coordinates) {
		return GetGrid((int) coordinates.x, (int) coordinates.y, (int) coordinates.z);
	}

	public Grid GetGrid(int x, int y, int z) {
		return grids[x, y, z];
	}

	public void SetGrid(Vector3 coordinates, Grid grid) {
		SetGrid((int) coordinates.x, (int) coordinates.y, (int) coordinates.z, grid);
	}

	public void SetGrid(int x, int y, int z, Grid grid) {
		grids[x, y, z] = grid;
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
	public IEnumerator GenerateGrids(int xDimension, int yDimension, int zDimension) {
		UIManager.Instance.Prompt("Generating grids...");

		siteDimensions = new Vector3(xDimension, yDimension, zDimension);

		grids = new Grid[(int) siteDimensions.x + 2, (int) siteDimensions.y + 2, (int) siteDimensions.z + 2];

		for (int y = 0; y < siteDimensions.y + 2; y++) {
			for (int x = 0; x < siteDimensions.x + 2; x++) {
				for (int z = 0; z < siteDimensions.z + 2; z++) {
					gridBuilder.BuildGrid(x, y, z, Grid.GridState.EMPTY);
				}
			}
		}

		for (int y = 1; y <= siteDimensions.y; y++) {
			for (int x = 1; x <= siteDimensions.x; x++) {
				for (int z = 1; z <= siteDimensions.z; z++) {
					grids[x, y, z].State = Grid.GridState.BASE;
				}
			}

			yield return new WaitForSeconds(0.05f);
		}

		for (int y = (int) siteDimensions.y; y >= 1; y--) {
			for (int x = 1; x <= siteDimensions.x; x++) {
				for (int z = 1; z <= siteDimensions.z; z++) {
					grids[x, y, z].State = Grid.GridState.EMPTY;
				}
			}

			yield return new WaitForSeconds(0.05f);
		}

		LinkGrids();
	}

	public SiteDimensionsMetadata GenerateMetadata() {
		return new SiteDimensionsMetadata(siteDimensions);
	}

	/*
	 * This method finds the closest grid to a given point in 3D space.
	 */
	public Grid GetClosestGrid(Vector3 point) {
		Collider[] hitColliders = Physics.OverlapSphere(
			point, 
			gridBuilder.gridFactory.innerGridPrefab.transform.localScale.x, 
			LayerUtils.Mask.EMPTY_GRID | LayerUtils.Mask.BASE_GRID);

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

	private void LinkGrids() {
		for (int y = 1; y <= siteDimensions.y; y++) {
			for (int x = 1; x <= siteDimensions.x; x++) {
				for (int z = 1; z <= siteDimensions.z; z++) {
					if (x != 1) {
						grids[x, y, z].SetNeighbour(Vector3.left, grids[x - 1, y, z]);
						grids[x - 1, y, z].SetNeighbour(Vector3.right, grids[x, y, z]);
					}

					if (y != 1) {
						grids[x, y, z].SetNeighbour(Vector3.down, grids[x, y - 1, z]);
						grids[x, y - 1, z].SetNeighbour(Vector3.up, grids[x, y, z]);
					}

					if (z != 1) {
						grids[x, y, z].SetNeighbour(Vector3.back, grids[x, y, z - 1]);
						grids[x, y, z - 1].SetNeighbour(Vector3.forward, grids[x, y, z]);
					}
				}
			}
		}
	}

}
