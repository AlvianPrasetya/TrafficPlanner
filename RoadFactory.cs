using UnityEngine;

public class RoadFactory : MonoBehaviour {

	public StreetPreview streetPreviewPrefab;
	public RampPreview rampPreviewPrefab;
	public OverpassPreview overpassPreviewPrefab;

	public Street streetPrefab;
	public Ramp rampPrefab;
	public Overpass overpassPrefab;
	public EntryRoad entryRoadPrefab;
	public ExitRoad exitRoadPrefab;

	public RoadPreview GetRoadPreviewPrefab(Grid startGrid, Grid endGrid) {
		if (startGrid == null && endGrid == null) {
			return null;
		}

		if (startGrid == null) {
			return null;
		}

		if (endGrid == null) {
			return streetPreviewPrefab;
		}

		if (startGrid.Coordinates.y == endGrid.Coordinates.y) {
			if (startGrid.Coordinates.y == 1) {
				return streetPreviewPrefab;
			} else {
				return overpassPreviewPrefab;
			}
		} else {
			return rampPreviewPrefab;
		}
	}

	public Road GetRoadPrefab(Grid startGrid, Grid endGrid) {
		if (startGrid == null && endGrid == null) {
			return null;
		}

		if (startGrid == null) {
			return null;
		}

		if (endGrid == null) {
			return streetPrefab;
		}

		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;

		if (startGrid.Coordinates.y == endGrid.Coordinates.y) {
			if (startGrid.Coordinates.x == 0 || startGrid.Coordinates.x == siteDimensions.x + 1 
				|| startGrid.Coordinates.z == 0 || startGrid.Coordinates.z == siteDimensions.z + 1) {
				return entryRoadPrefab;
			} else if (endGrid.Coordinates.x == 0 || endGrid.Coordinates.x == siteDimensions.x + 1 
				|| endGrid.Coordinates.z == 0 || endGrid.Coordinates.z == siteDimensions.z + 1) {
				return exitRoadPrefab;
			} else if (startGrid.Coordinates.y == 1) {
				return streetPrefab;
			} else {
				return overpassPrefab;
			}
		} else {
			return rampPrefab;
		}
	}

}
