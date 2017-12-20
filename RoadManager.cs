using UnityEngine;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour {

	public RoadBuilder roadBuilder;

	private HashSet<Road> roads;
	private Dictionary<Grid, HashSet<Road>> roadsStartingAt;
	private Dictionary<Grid, HashSet<Road>> roadsEndingAt;

	public IEnumerable<Road> Roads {
		get {
			return roads;
		}
	}

	public void AddRoad(Road road) {
		roads.Add(road);
	}

	public void RemoveRoad(Road road) {
		roads.Remove(road);
	}

	public IEnumerable<Road> GetRoadsStartingAt(Grid startGrid) {
		HashSet<Road> roadSet;
		if (roadsStartingAt.TryGetValue(startGrid, out roadSet)) {
			return roadSet;
		} else {
			return null;
		}
	}

	public IEnumerable<Road> GetRoadsEndingAt(Grid endGrid) {
		HashSet<Road> roadSet;
		if (roadsEndingAt.TryGetValue(endGrid, out roadSet)) {
			return roadSet;
		} else {
			return null;
		}
	}

	public void AddRoadStartingAt(Grid startGrid, Road road) {
		HashSet<Road> roadSet;
		if (!roadsStartingAt.TryGetValue(startGrid, out roadSet)) {
			roadSet = new HashSet<Road>();
			roadsStartingAt.Add(startGrid, roadSet);
		}

		roadSet.Add(road);
	}

	public void RemoveRoadStartingAt(Grid startGrid, Road road) {
		HashSet<Road> roadSet;
		if (roadsStartingAt.TryGetValue(startGrid, out roadSet)) {
			roadSet.Remove(road);
		} else {
			// Error: roads starting at the given grid is not found
			Debug.Log("Error: roads starting at the given grid is not found");
		}
	}

	public void AddRoadEndingAt(Grid endGrid, Road road) {
		HashSet<Road> roadSet;
		if (!roadsEndingAt.TryGetValue(endGrid, out roadSet)) {
			roadSet = new HashSet<Road>();
			roadsEndingAt.Add(endGrid, roadSet);
		}

		roadSet.Add(road);
	}

	public void RemoveRoadEndingAt(Grid endGrid, Road road) {
		HashSet<Road> roadSet;
		if (roadsEndingAt.TryGetValue(endGrid, out roadSet)) {
			roadSet.Remove(road);
		} else {
			// Error: roads starting at the given grid is not found
		}
	}

	public void GenerateAccessPoints(List<TrafficMetadata> trafficMetadataList) {
		foreach (TrafficMetadata trafficMetadata in trafficMetadataList) {
			Grid startGrid, endGrid;

			Vector3 entryCoordinates = trafficMetadata.EntryCoordinates;
			GetEntryRoadStartEndGrids(entryCoordinates, out startGrid, out endGrid);
			EntryRoad entryRoad = (EntryRoad) roadBuilder.BuildRoad(startGrid, endGrid);

			List<ExitRoad> exitRoads = new List<ExitRoad>();
			foreach (Vector3 exitCoordinates in trafficMetadata.ExitCoordinateList) {
				GetExitRoadStartEndGrids(exitCoordinates, out startGrid, out endGrid);
				exitRoads.Add((ExitRoad) roadBuilder.BuildRoad(startGrid, endGrid));
			}

			SiteManager.Instance.trafficManager.AddTraffic(
				entryRoad, exitRoads, 
				trafficMetadata.TrafficVolumeList, trafficMetadata.TrafficVolume);
		}
	}

	private void Awake() {
		roads = new HashSet<Road>();
		roadsStartingAt = new Dictionary<Grid, HashSet<Road>>();
		roadsEndingAt = new Dictionary<Grid, HashSet<Road>>();
	}

	private void GetEntryRoadStartEndGrids(Vector3 entryCoordinates, out Grid startGrid, out Grid endGrid) {
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;
		Grid[,,] grids = SiteManager.Instance.gridManager.Grids;

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

		startGrid = grids[(int) startGridCoordinates.x, (int) startGridCoordinates.y, (int) startGridCoordinates.z];
		endGrid = grids[(int) endGridCoordinates.x, (int) endGridCoordinates.y, (int) endGridCoordinates.z];
		return;
	}

	private void GetExitRoadStartEndGrids(Vector3 exitCoordinates, out Grid startGrid, out Grid endGrid) {
		Vector3 siteDimensions = SiteManager.Instance.gridManager.SiteDimensions;
		Grid[,,] grids = SiteManager.Instance.gridManager.Grids;

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

		startGrid = grids[(int) startGridCoordinates.x, (int) startGridCoordinates.y, (int) startGridCoordinates.z];
		endGrid = grids[(int) endGridCoordinates.x, (int) endGridCoordinates.y, (int) endGridCoordinates.z];
		return;
	}

}
