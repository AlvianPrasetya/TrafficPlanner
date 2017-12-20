using UnityEngine;
using System.Collections.Generic;

public class TrafficManager : MonoBehaviour {

	private List<Traffic> traffics;
	private List<float> nextSpawnTimes;

	public void AddTraffic(EntryRoad entryRoad, IEnumerable<ExitRoad> exitRoads, 
		IEnumerable<float> trafficDistributions, int trafficVolume) {
		traffics.Add(new Traffic(entryRoad, exitRoads, trafficDistributions, trafficVolume));
	}

	public void PrepareSimulation() {
		nextSpawnTimes = new List<float>();
		foreach (Traffic traffic in traffics) {
			traffic.Initialize();
			nextSpawnTimes.Add(0.0f);
		}
	}

	public void SimulateAtTime(float currentSimulationTime) {
		for (int i = 0; i < traffics.Count; i++) {
			if (currentSimulationTime > nextSpawnTimes[i]) {
				Traffic.Path path = traffics[i].GetRandomTrafficRoute().GetRandomPath();
				// TODO: Spawn vehicle
				nextSpawnTimes[i] += 1.0f / traffics[i].TrafficVolume;
			}
		}
	}

	private void Awake() {
		traffics = new List<Traffic>();
	}

	/*private struct TrafficFlow {

		public Grid entryGrid;
		public Grid exitGrid;
		public int trafficRate;

		public TrafficFlow(Vector3 entryCoordinates, Vector3 exitCoordinates, int trafficRate) {
			entryGrid = SiteManager.Instance.gridManager.Grids[
				(int) entryCoordinates.x, 
				(int) entryCoordinates.y, 
				(int) entryCoordinates.z];
			exitGrid = SiteManager.Instance.gridManager.Grids[
				(int) exitCoordinates.x,
				(int) exitCoordinates.y,
				(int) exitCoordinates.z];
			;
			this.trafficRate = trafficRate;
		}

	}

	private static readonly char TRAFFIC_FLOWS_SEPARATOR = '/';
	private static readonly char TRAFFIC_FLOW_INFO_SEPARATOR = '_';
	private static readonly char TRAFFIC_COORDINATES_SEPARATOR = '-';

	public VehicleController carPrefab;

	private List<TrafficFlow> trafficFlows;
	private float[] nextTrafficSpawnTime;
	private List<VehicleController> simulatedVehicles;

	public void ParseTrafficFlowsMetadata(string trafficFlowsMetadata) {
		string[] trafficFlowTokens = trafficFlowsMetadata.Split(TRAFFIC_FLOWS_SEPARATOR);

		foreach (string trafficFlowToken in trafficFlowTokens) {
			string[] trafficFlowInfoTokens = trafficFlowToken.Split(TRAFFIC_FLOW_INFO_SEPARATOR);

			string[] entryPointTokens = trafficFlowInfoTokens[0].Split(TRAFFIC_COORDINATES_SEPARATOR);
			string[] exitPointTokens = trafficFlowInfoTokens[1].Split(TRAFFIC_COORDINATES_SEPARATOR);
			// The rate of traffic in # of cars per minute
			int trafficRate = int.Parse(trafficFlowInfoTokens[2]);

			Vector3 entryCoordinate = new Vector3(
					int.Parse(entryPointTokens[0]),
					int.Parse(entryPointTokens[1]),
					int.Parse(entryPointTokens[2]));

			Vector3 exitCoordinate = new Vector3(
				int.Parse(exitPointTokens[0]),
				int.Parse(exitPointTokens[1]),
				int.Parse(exitPointTokens[2]));

			trafficFlows.Add(new TrafficFlow(entryCoordinate, exitCoordinate, trafficRate));
		}

		nextTrafficSpawnTime = new float[trafficFlows.Count];
	}

	public void simulateAtTime(float currentSimulationTime) {
		for (int i = 0; i < trafficFlows.Count; i++) {
			if (currentSimulationTime >= nextTrafficSpawnTime[i]) {
				VehicleController car = Instantiate(
					carPrefab,
					trafficFlows[i].entryGrid.transform.position
					+ Vector3.up * 0.2f,
					Quaternion.identity);
				car.Initialize(trafficFlows[i].entryGrid, trafficFlows[i].exitGrid);
				simulatedVehicles.Add(car);
				nextTrafficSpawnTime[i] += 60.0f / trafficFlows[i].trafficRate;
			}
		}
	}

	public void ResetSimulation() {
		Array.Clear(nextTrafficSpawnTime, 0, nextTrafficSpawnTime.Length);
		foreach (VehicleController vehicle in simulatedVehicles) {
			if (vehicle != null) {
				Destroy(vehicle.gameObject);
			}
		}
		simulatedVehicles.Clear();
	}

	private void Awake() {
		trafficFlows = new List<TrafficFlow>();
		simulatedVehicles = new List<VehicleController>();
	}*/

}
