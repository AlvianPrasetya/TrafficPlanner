using UnityEngine;

public class RoadFactory : MonoBehaviour {

	// Road Previews
	public RoadPreview roadPreviewPrefab;

	// Access Roads
	public EntryRoad entryRoadPrefab;
	public ExitRoad exitRoadPrefab;

	// Arterials
	public Road arterialPrefab;

	// Roads
	public Road streetPrefab;
	public Road rampPrefab;
	public Road overpassPrefab;

	public RoadPreview GetRoadPreviewPrefab() {
		return roadPreviewPrefab;
	}

	public EntryRoad GetEntryRoadPrefab() {
		return entryRoadPrefab;
	}

	public ExitRoad GetExitRoadPrefab() {
		return exitRoadPrefab;
	}

	public Road GetArterialPrefab() {
		return arterialPrefab;
	}

	public Road GetRoadPrefab(Grid startGrid, Grid endGrid) {
		if (startGrid == null || endGrid == null) {
			return null;
		}

		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;

		if (startGrid.Coordinates.y == 1 && endGrid.Coordinates.y == 1) {
			return streetPrefab;
		} else if (startGrid.Coordinates.y == 1 || endGrid.Coordinates.y == 1) {
			return rampPrefab;
		} else {
			return overpassPrefab;
		}
	}

}
