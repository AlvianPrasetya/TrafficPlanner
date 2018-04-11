using UnityEngine;

public class GridFactory : MonoBehaviour {
	
	public Grid innerGridPrefab;
	public Grid outerGridPrefab;

	public Grid GetGridPrefab(int x, int y, int z) {
		if (x == 0 || x == SiteManager.Instance.gridManager.SiteDimensions.x + 1
			|| y == 0 || y == SiteManager.Instance.gridManager.SiteDimensions.y + 1
			|| z == 0 || z == SiteManager.Instance.gridManager.SiteDimensions.z + 1) {
			return outerGridPrefab;
		}

		return innerGridPrefab;
	}

}
