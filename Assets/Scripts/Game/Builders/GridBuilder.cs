using UnityEngine;

public class GridBuilder : MonoBehaviour {

	public GridFactory gridFactory;

	public void BuildGrid(int x, int y, int z, Grid.GridState initialState) {
		Grid gridPrefab = gridFactory.GetGridPrefab(x, y, z);

		Vector3 gridPosition = new Vector3(
			(x - SiteManager.Instance.gridManager.SiteDimensions.x / 2.0f - 0.5f) * gridPrefab.transform.localScale.x,
			(y - 1.0f) * gridPrefab.transform.localScale.y,
			(z - SiteManager.Instance.gridManager.SiteDimensions.z / 2.0f - 0.5f) * gridPrefab.transform.localScale.z);
		Quaternion gridRotation = Quaternion.identity;

		Grid grid = Instantiate(gridPrefab, gridPosition, gridRotation, SiteManager.Instance.gridManager.transform);
		grid.State = initialState;
		grid.Coordinates = new Vector3(x, y, z);
	}

}
