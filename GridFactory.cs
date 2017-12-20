using UnityEngine;

public class GridFactory : MonoBehaviour {
	
	public Grid innerGridPrefab;
	public Grid outerGridPrefab;

	public Grid GetGridPrefab(int x, int y, int z) {
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;

		if (x == 0 || x == (int) siteDimensions.x + 1
			|| y == 0 || y == (int) siteDimensions.y + 1
			|| z == 0 || z == (int) siteDimensions.z + 1) {
			return outerGridPrefab;
		}

		return innerGridPrefab;
	}

}
