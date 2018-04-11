using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TrafficManager : MonoBehaviour {

	public TrafficBuilder trafficBuilder;

	public VehicleFactory vehicleFactory;

	private Dictionary<EntryRoad, List<Traffic>> trafficsStartingAt;
	private Dictionary<ExitRoad, List<Traffic>> trafficsEndingAt;

	private Dictionary<EntryRoad, float> nextSpawnTimes;
	private Dictionary<EntryRoad, LinkedListNode<Traffic>> nextSpawnTraffics;

	private HashSet<VehicleController> simulatedVehicles;

	public void AddTraffic(Traffic traffic) {
		if (!trafficsStartingAt.ContainsKey(traffic.EntryRoad)) {
			trafficsStartingAt[traffic.EntryRoad] = new List<Traffic>();
		}
		trafficsStartingAt[traffic.EntryRoad].Add(traffic);

		if (!trafficsEndingAt.ContainsKey(traffic.ExitRoad)) {
			trafficsEndingAt[traffic.ExitRoad] = new List<Traffic>();
		}
		trafficsEndingAt[traffic.ExitRoad].Add(traffic);
	}

	public void RemoveTraffic(Traffic traffic) {
		trafficsStartingAt[traffic.EntryRoad].Remove(traffic);
		if (trafficsStartingAt[traffic.EntryRoad].Count == 0) {
			traffic.EntryRoad.Demolish();
		}

		trafficsEndingAt[traffic.ExitRoad].Remove(traffic);
		if (trafficsEndingAt[traffic.ExitRoad].Count == 0) {
			traffic.ExitRoad.Demolish();
		}
	}

	public void RemoveEntryRoad(EntryRoad entryRoad) {
		// Make a shallow copy of trafficsStartingAt
		List<Traffic> trafficsStartingAtCopy = new List<Traffic>(trafficsStartingAt[entryRoad]);
		foreach (Traffic traffic in trafficsStartingAtCopy) {
			trafficsStartingAt[traffic.EntryRoad].Remove(traffic);

			trafficsEndingAt[traffic.ExitRoad].Remove(traffic);
			traffic.ExitRoad.accessPointInfographic.RemoveTraffic(traffic);
			if (trafficsEndingAt[traffic.ExitRoad].Count == 0) {
				traffic.ExitRoad.Demolish();
			}
		}
	}

	public void RemoveExitRoad(ExitRoad exitRoad) {
		// Make a shallow copy of trafficsEndingAt
		List<Traffic> trafficsEndingAtCopy = new List<Traffic>(trafficsEndingAt[exitRoad]);
		foreach (Traffic traffic in trafficsEndingAtCopy) {
			trafficsStartingAt[traffic.EntryRoad].Remove(traffic);
			traffic.EntryRoad.accessPointInfographic.RemoveTraffic(traffic);
			if (trafficsStartingAt[traffic.EntryRoad].Count == 0) {
				traffic.EntryRoad.Demolish();
			}

			trafficsEndingAt[traffic.ExitRoad].Remove(traffic);
		}
	}

	public void AddSimulatedVehicle(VehicleController vehicle) {
		simulatedVehicles.Add(vehicle);
	}

	public void RemoveSimulatedVehicle(VehicleController vehicle) {
		simulatedVehicles.Remove(vehicle);
	}

	public IEnumerator GenerateAccessPoints(List<TrafficMetadata> trafficMetadataList) {
		UIManager.Instance.Prompt("Generating access points...");

		foreach (TrafficMetadata trafficMetadata in trafficMetadataList) {
			trafficBuilder.BuildTraffic(
				SiteManager.Instance.gridManager.GetGrid(trafficMetadata.EntryCoordinates),
				SiteManager.Instance.gridManager.GetGrid(trafficMetadata.ExitCoordinates),
				trafficMetadata.TrafficVolume);

			yield return null;
		}
	}

	public List<TrafficMetadata> GenerateMetadata() {
		List<TrafficMetadata> trafficMetadataList = new List<TrafficMetadata>();
		foreach (KeyValuePair<EntryRoad, List<Traffic>> trafficStartingAt in trafficsStartingAt) {
			foreach (Traffic traffic in trafficStartingAt.Value) {
				trafficMetadataList.Add(new TrafficMetadata(
					traffic.EntryRoad.EndGrid.Coordinates,
					traffic.ExitRoad.StartGrid.Coordinates,
					traffic.TrafficVolume));
			}
		}

		return trafficMetadataList;
	}

	/**
	 * This method prepares for simulation by initializing all traffic paths.
	 * Returns true if all paths could be generated, false if any path could not be generated due to unreachability.
	 */
	public bool PrepareSimulation() {
		bool preparationSuccess = true;

		nextSpawnTimes = new Dictionary<EntryRoad, float>();
		nextSpawnTraffics = new Dictionary<EntryRoad, LinkedListNode<Traffic>>();

		foreach (KeyValuePair<EntryRoad, List<Traffic>> trafficStartingAt in trafficsStartingAt) {
			// Generate paths for each traffic, spawn times and spawn order for each commonly starting traffic
			LinkedList<Traffic> spawnOrders = new LinkedList<Traffic>();
			foreach (Traffic traffic in trafficStartingAt.Value) {
				preparationSuccess &= traffic.GeneratePaths();
				spawnOrders = new LinkedList<Traffic>(spawnOrders.Concat(
					Enumerable.Repeat(traffic, Mathf.CeilToInt(traffic.TrafficVolume * 24.0f))));
			}
			
			spawnOrders = new LinkedList<Traffic>(spawnOrders.OrderBy(i => Random.value));

			nextSpawnTimes[trafficStartingAt.Key] = 0.0f;
			nextSpawnTraffics[trafficStartingAt.Key] = spawnOrders.First;
		}

		return preparationSuccess;
	}

	/**
	 * This method simulates traffic spawning at the given in-game time.
	 */
	public void SimulateAtTime(float inGameTime) {
		// Calculate estimated relative traffic volume by averaging the relative traffic volume throughout the hour
		float lbRelativeTrafficVolume = CalculateRelativeTrafficVolumeAtTime(Mathf.FloorToInt(inGameTime));
		float ubRelativeTrafficVolume = CalculateRelativeTrafficVolumeAtTime(Mathf.RoundToInt(inGameTime + 0.5f));
		float relativeTrafficVolume = (lbRelativeTrafficVolume + ubRelativeTrafficVolume) / 2.0f;

		foreach (KeyValuePair<EntryRoad, List<Traffic>> trafficStartingAt in trafficsStartingAt) {
			if (inGameTime >= nextSpawnTimes[trafficStartingAt.Key]) {
				Path path = nextSpawnTraffics[trafficStartingAt.Key].Value.GetRandomPath();

				LinkedListNode<Road> firstRoadNode = path.Roads.First;

				VehicleController vehicle = Instantiate(
					vehicleFactory.GetRandomVehicle(),
					firstRoadNode.Value.StartGrid.transform.position,
					firstRoadNode.Value.transform.rotation);
				vehicle.Initialize(firstRoadNode);

				// Add simulation manager as the speed observer
				vehicle.AddObserver(SiteManager.Instance.simulationManager);

				// Increment spawn time and move traffic iterator
				nextSpawnTimes[trafficStartingAt.Key] += 1.0f
					/ (relativeTrafficVolume * trafficStartingAt.Value.Sum(i => i.TrafficVolume));
				nextSpawnTraffics[trafficStartingAt.Key] = nextSpawnTraffics[trafficStartingAt.Key].Next;
			}
		}
	}

	public void ResetSimulation() {
		foreach (VehicleController vehicle in simulatedVehicles) {
			Destroy(vehicle.gameObject);
		}
	}

	private void Awake() {
		trafficsStartingAt = new Dictionary<EntryRoad, List<Traffic>>();
		trafficsEndingAt = new Dictionary<ExitRoad, List<Traffic>>();

		simulatedVehicles = new HashSet<VehicleController>();
	}

	private float CalculateRelativeTrafficVolumeAtTime(float inGameTime) {
		return 1.5611f * Mathf.Exp(-0.04f * Mathf.Pow(inGameTime - 8.0f, 2))
			+ 1.5611f * Mathf.Exp(-0.07f * Mathf.Pow(inGameTime - 18.0f, 2));
	}

}
