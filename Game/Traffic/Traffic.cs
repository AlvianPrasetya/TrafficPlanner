using System;
using System.Collections.Generic;
using UnityEngine;

/**
* Traffic is defined by a pair of entry road and exit road with a defined
* traffic volume (number of vehicles going through per in-game hour).
*/
public class Traffic : ICollectible {

	private EntryRoad entryRoad;
	private ExitRoad exitRoad;
	private float trafficVolume;
	private List<Path> paths;

	public void Enlist() {
		SiteManager.Instance.trafficManager.AddTraffic(this);
		entryRoad.accessPointInfographic.AddTraffic(this);
		exitRoad.accessPointInfographic.AddTraffic(this);
	}

	public void Delist() {
		throw new NotImplementedException("Traffic ICollectible could not be delisted");
	}

	public Traffic(EntryRoad entryRoad, ExitRoad exitRoad, float trafficVolume) {
		this.entryRoad = entryRoad;
		this.exitRoad = exitRoad;
		this.trafficVolume = trafficVolume;

		Enlist();
	}

	public EntryRoad EntryRoad {
		get {
			return entryRoad;
		}
	}

	public ExitRoad ExitRoad {
		get {
			return exitRoad;
		}
	}

	public float TrafficVolume {
		get {
			return trafficVolume;
		}
	}

	/**
	 * This method generates paths pertaining to this TrafficRoute.
	 * Returns true if at least 1 path is successfully calculated, false otherwise.
	 */
	public bool GeneratePaths() {
		paths = new List<Path>();
		DFSPaths(entryRoad, exitRoad, new HashSet<Grid>(), new Path());

		// Sort by path length
		paths.Sort((x, y) => x.PathLength.CompareTo(y.PathLength));

		return paths.Count > 0;
	}

	/**
	 * This method gets a weighted random path dependent on the square of relative path length.
	 * For example, with 4 paths of length 1, 2, 3, and 4:
	 * Path of length 1 will be selected with probability of 16 / 22.78
	 * Path of length 2 will be selected with probability of 4 / 22.78
	 * Path of length 3 will be selected with probability of 1.78 / 22.78
	 * Path of length 4 will be selected with probability of 1 / 22.78
	 */
	public Path GetRandomPath() {
		float maxPathLength = 0.0f;
		foreach (Path path in paths) {
			maxPathLength = Mathf.Max(maxPathLength, path.PathLength);
		}

		float weightedRelativeSqrSum = 0.0f;
		foreach (Path path in paths) {
			weightedRelativeSqrSum += Mathf.Pow(maxPathLength / path.PathLength, 2);
		}

		float randomFloat = UnityEngine.Random.Range(0.0f, weightedRelativeSqrSum);
		foreach (Path path in paths) {
			randomFloat -= Mathf.Pow(maxPathLength / path.PathLength, 2);
			if (randomFloat <= 0.0f) {
				return path;
			}
		}

		return null;
	}

	private void DFSPaths(Road source, Road destination, HashSet<Grid> visitedGrids, Path path) {
		visitedGrids.Add(source.StartGrid);
		path.Append(source);

		if (source == destination) {
			paths.Add(new Path(path));
		} else {
			foreach (Road outgoingRoad in source.OutgoingRoads) {
				if (!visitedGrids.Contains(outgoingRoad.EndGrid)) {
					DFSPaths(outgoingRoad, destination, visitedGrids, path);
				}
			}
		}

		path.Truncate();
		visitedGrids.Remove(source.StartGrid);
	}

}
