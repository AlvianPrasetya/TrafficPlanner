using UnityEngine;
using System.Collections;

public class SiteManager : MonoBehaviour {

	public struct SimulationStats {

		private float simulationTime;
		private int numVehiclesSpawned;
		private int numVehiclesReachedExit;

		public SimulationStats(float simulationTime) {
			this.simulationTime = simulationTime;
			numVehiclesSpawned = 0;
			numVehiclesReachedExit = 0;
		}
		
		public int NumVehiclesSpawned {
			get {
				return numVehiclesSpawned;
			}
		}

		public int NumVehiclesReachedExit {
			get {
				return numVehiclesReachedExit;
			}
		}

		public float FractionReachedExit {
			get {
				return (float) numVehiclesReachedExit / numVehiclesSpawned;
			}
		}

		public void AddVehicleSpawned() {
			numVehiclesSpawned++;
		}

		public void AddVehicleReachedExit() {
			numVehiclesReachedExit++;
		}

	}

	public GridManager gridManager;
	public LandmarkManager landmarkManager;
	public RoadManager roadManager;
	public TrafficManager trafficManager;
	public LandmarkAssetManager landmarkAssetManager;

	public Ground groundPrefab;

	public int simulationDuration;

	private static SiteManager instance;

	private bool simulating;
	private float currentSimulationTime;
	private SimulationStats simulationStats;

	public static SiteManager Instance {
		get {
			return instance;
		}
	}

	[Tooltip("The metadata that describes this level uniquely")]
	public string loadedMetadata;

	public void Simulate() {
		Debug.Log("Preparing simulation...");
		//gridManager.RecalculateShortestPaths();
		trafficManager.PrepareSimulation();

		Debug.Log("Simulation started");
		//simulating = true;
		//StartCoroutine(SimulateCoroutine());
	}

	public void OnVehicleSpawnedCallback() {
		simulationStats.AddVehicleSpawned();
	}

	public void OnVehicleReachedExitCallback() {
		simulationStats.AddVehicleReachedExit();
	}

	private void Awake() {
		instance = this;

		simulating = false;
	}

	private void Start() {
		LevelMetadata levelMetadata = new LevelMetadata(loadedMetadata);

		GenerateGround(levelMetadata.SiteDimensionsMetadata);
		gridManager.GenerateGrids(levelMetadata.SiteDimensionsMetadata);
		landmarkManager.GenerateLandmarks(levelMetadata.LandmarkMetadataList);
		roadManager.GenerateAccessPoints(levelMetadata.AccessPointMetadataList);
	}

	private void GenerateGround(SiteDimensionsMetadata siteDimensionsMetadata) {
		Ground ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity);
		ground.Scale = new Vector3(
			siteDimensionsMetadata.SiteDimensions.x, 
			1.0f, 
			siteDimensionsMetadata.SiteDimensions.z);
	}

	private IEnumerator SimulateCoroutine() {
		UIManager.Instance.Prompt("Simulation started");

		simulationStats = new SimulationStats(currentSimulationTime);

		currentSimulationTime = 0.0f;
		while (currentSimulationTime < simulationDuration) {
			trafficManager.SimulateAtTime(currentSimulationTime);

			currentSimulationTime += Time.deltaTime;
			yield return null;
		}
		
		trafficManager.PrepareSimulation();

		UIManager.Instance.OpenOverlay(
			"<b>Simulation Results</b>\n\n"
			+ "Simulation time: " + string.Format("{0:0.##} seconds", simulationDuration) + "\n"
			+ "Vehicles spawned: " + simulationStats.NumVehiclesSpawned + "\n"
			+ "Vehicles reached: " + simulationStats.NumVehiclesReachedExit + " (" + string.Format("{0:0.##}%", 100 * simulationStats.FractionReachedExit) + ")\n", 
			UIManager.Instance.CloseOverlay, "Back to Planning", 
			UIManager.Instance.CloseOverlay, "Main Menu");
	}

}
