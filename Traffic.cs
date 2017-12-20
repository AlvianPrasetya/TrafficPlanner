using UnityEngine;
using System.Collections.Generic;

public class Traffic {

	public class TrafficRoute {

		private EntryRoad entryRoad;
		private ExitRoad exitRoad;
		private float trafficDistribution;
		private List<Path> paths;

		public TrafficRoute(EntryRoad entryRoad, ExitRoad exitRoad, float trafficDistribution) {
			this.entryRoad = entryRoad;
			this.exitRoad = exitRoad;
			this.trafficDistribution = trafficDistribution;
		}

		public float TrafficDistribution {
			get {
				return trafficDistribution;
			}
		}

		public void CalculatePaths() {
			paths = new List<Path>();
			DFSPaths(entryRoad, exitRoad, new HashSet<Grid>(), new Path());

			// Sort by path length
			paths.Sort((x, y) => x.PathLength.CompareTo(y.PathLength));
		}

		public Path GetRandomPath() {
			float totalPathLength = 0.0f;
			foreach (Path path in paths) {
				totalPathLength += path.PathLength;
			}

			float[] cumulativeProbability = new float[paths.Count];
			cumulativeProbability[0] = paths[paths.Count - 1].PathLength / totalPathLength;
			for (int i = 1; i < paths.Count; i++) {
				cumulativeProbability[i] = cumulativeProbability[i - 1] + paths[paths.Count - 1 - i].PathLength / totalPathLength;
				Debug.Log(cumulativeProbability[i]);
			}

			float randomFloat = Random.value;
			for (int i = 0; i < cumulativeProbability.Length; i++) {
				if (cumulativeProbability[i] > randomFloat) {
					return paths[i];
				}
			}

			return null;
		}

		private void DFSPaths(Road source, ExitRoad destination, HashSet<Grid> visitedGrids, Path path) {
			visitedGrids.Add(source.StartGrid);
			path.Append(source);

			if (source == destination) {
				paths.Add(path);
				string log = path.PathLength + "";
				foreach (Road road in path.Roads) {
					log += " -> " + road.StartGrid.Coordinates + " " + road.EndGrid.Coordinates;
				}
				Debug.Log(log);
			} else {
				foreach (Road outgoingRoad in source.outgoingRoads) {
					if (!visitedGrids.Contains(outgoingRoad.EndGrid)) {
						DFSPaths(outgoingRoad, destination, visitedGrids, path);
					}
				}
			}

			path.Truncate();
			visitedGrids.Remove(source.StartGrid);
		}

	}

	public class Path {

		private LinkedList<Road> roads;
		private float pathLength;
		
		public Path() {
			roads = new LinkedList<Road>();
			pathLength = 0;
		}

		public Path(Path otherPath) {
			roads = new LinkedList<Road>(otherPath.Roads);
			pathLength = otherPath.pathLength;
		}

		public IEnumerable<Road> Roads {
			get {
				return roads;
			}
		}

		public void Append(Road road) {
			roads.AddLast(road);
			pathLength += road.RoadLength;
		}

		public void Truncate() {
			pathLength -= roads.Last.Value.RoadLength;
			roads.RemoveLast();
		}

		public float PathLength {
			get {
				return pathLength;
			}
		}

	}

	private List<TrafficRoute> trafficRoutes;
	private int trafficVolume;

	public Traffic(EntryRoad entryRoad, IEnumerable<ExitRoad> exitRoads, 
		IEnumerable<float> trafficDistributions, int trafficVolume) {
		trafficRoutes = new List<TrafficRoute>();
		IEnumerator<ExitRoad> exitRoadEnumerator = exitRoads.GetEnumerator(); ;
		IEnumerator<float> trafficDistributionEnumerator = trafficDistributions.GetEnumerator();
		while (exitRoadEnumerator.MoveNext() && trafficDistributionEnumerator.MoveNext()) {
			trafficRoutes.Add(new TrafficRoute(entryRoad, exitRoadEnumerator.Current, 
				trafficDistributionEnumerator.Current));
		}
		exitRoadEnumerator.Reset();
		trafficDistributionEnumerator.Reset();

		// Sort traffic routes by their distribution value
		trafficRoutes.Sort((x, y) => x.TrafficDistribution.CompareTo(y.TrafficDistribution));

		this.trafficVolume = trafficVolume;
	}

	public IEnumerable<TrafficRoute> TrafficRoutes {
		get {
			return trafficRoutes;
		}
	}

	public int TrafficVolume {
		get {
			return trafficVolume;
		}
	}

	public void Initialize() {
		foreach (TrafficRoute trafficRoute in trafficRoutes) {
			trafficRoute.CalculatePaths();
		}
	}

	public TrafficRoute GetRandomTrafficRoute() {
		float totalDistribution = 0.0f;
		foreach (TrafficRoute trafficRoute in trafficRoutes) {
			totalDistribution += trafficRoute.TrafficDistribution;
		}

		float[] cumulativeProbability = new float[trafficRoutes.Count];
		cumulativeProbability[0] = trafficRoutes[trafficRoutes.Count - 1].TrafficDistribution / totalDistribution;
		for (int i = 1; i < trafficRoutes.Count; i++) {
			cumulativeProbability[i] = cumulativeProbability[i - 1] 
				+ trafficRoutes[trafficRoutes.Count - 1 - i].TrafficDistribution / totalDistribution;
			Debug.Log(cumulativeProbability[i]);
		}

		float randomFloat = Random.value;
		for (int i = 0; i < cumulativeProbability.Length; i++) {
			if (cumulativeProbability[i] > randomFloat) {
				return trafficRoutes[i];
			}
		}

		return null;
	}

}
