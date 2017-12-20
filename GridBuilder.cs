using UnityEngine;

public class GridBuilder : MonoBehaviour {

	public GridFactory gridFactory;

	public void BuildGrid(int x, int y, int z) {
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;

		Grid gridPrefab = gridFactory.GetGridPrefab(x, y, z);

		Vector3 gridPosition = new Vector3(
			(x - siteDimensions.x / 2.0f - 0.5f) * gridPrefab.transform.localScale.x,
			(y - 1.0f) * gridPrefab.transform.localScale.y,
			(z - siteDimensions.z / 2.0f - 0.5f) * gridPrefab.transform.localScale.z);
		Quaternion gridRotation = Quaternion.identity;

		Grid grid = Instantiate(gridPrefab, gridPosition, gridRotation, 
			SiteManager.Instance.gridManager.transform);
		grid.State = Grid.GridState.EMPTY;
		grid.Coordinates = new Vector3(x, y, z);
	}

}
