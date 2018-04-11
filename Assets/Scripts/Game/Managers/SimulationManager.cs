using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : Observer<VehicleController, float> {

	public class SimulationStats {

		private static readonly float MAX_VEHICLE_SPEED = 4.0f;

		private float totalSimulationDuration;
		private int numVehiclesSpawned;
		private int numVehiclesReachedExit;

		private Dictionary<VehicleController, Averager> observedAverageSpeeds;

		public SimulationStats() {
			totalSimulationDuration = 0.0f;
			numVehiclesSpawned = 0;
			numVehiclesReachedExit = 0;
			observedAverageSpeeds = new Dictionary<VehicleController, Averager>();
		}

		public float TotalSimulationTime {
			get {
				return totalSimulationDuration;
			}

			set {
				totalSimulationDuration = value;
			}
		}

		public int NumVehiclesSpawned {
			get {
				return numVehiclesSpawned;
			}

			set {
				numVehiclesSpawned = value;
			}
		}

		public int NumVehiclesReachedExit {
			get {
				return numVehiclesReachedExit;
			}

			set {
				numVehiclesReachedExit = value;
			}
		}

		public float AverageSpeed {
			get {
				float totalAverage = 0.0f;
				int numVehicles = 0;
				foreach (KeyValuePair<VehicleController, Averager> vehicleSpeed in observedAverageSpeeds) {
					totalAverage += vehicleSpeed.Value.Average;
					numVehicles++;
				}

				return totalAverage / numVehicles;
			}
		}

		public float TrafficQuality {
			get {
				return AverageSpeed / MAX_VEHICLE_SPEED;
			}
		}

		public float Budget {
			get {
				return BudgetManager.Instance.Budget;
			}
		}

		public float Score {
			get {
				return TrafficQuality * 100.0f / Budget;
			}
		}

		public void AddDataPoint(VehicleController vehicle, float speed) {
			if (!observedAverageSpeeds.ContainsKey(vehicle)) {
				observedAverageSpeeds.Add(vehicle, new Averager());
			}

			observedAverageSpeeds[vehicle].AddDataPoint(speed);
		}

	}

	private static readonly string ATTEMPT_RANK_DIR = "/attempt_rank.php";

	private static readonly string USERNAME_PARAM = "username";
	private static readonly string SESSION_ID_PARAM = "session_id";
	private static readonly string LEVEL_ID_PARAM = "level_id";
	private static readonly string SCORE_PARAM = "score";

	public float simulationTimescale;

	public TabGroup uiTabGroup;
	public int defaultTabId;
	public int simulationTabId;

	public Image clockDayBackground;
	public Image clockNightBackground;
	public Transform clockHand;
	public Slider trafficQualitySlider;

	public GameObject simulationOverlay;
	public Text simulationTime;
	public Text simulatedVehicles;
	public Text averageSpeed;
	public Text budgetRequired;
	public Text score;

	private SimulationStats simulationStats;
	private Coroutine simulationCoroutine;

	public override void Notify(VehicleController vehicle, float speed) {
		simulationStats.AddDataPoint(vehicle, speed);
	}

	public void StartSimulation() {
		Debug.Log("Preparing simulation...");
		UIManager.Instance.Prompt("Preparing simulation...");
		if (SiteManager.Instance.trafficManager.PrepareSimulation()) {
			Debug.Log("Simulation started");
			simulationCoroutine = StartCoroutine(SimulateCoroutine());
		} else {
			Debug.Log("One or more routes are not connected by roads");
			UIManager.Instance.Prompt("One or more routes are not connected by roads");
		}
	}

	public void StopSimulation() {
		Debug.Log("Stopping simulation...");
		UIManager.Instance.Prompt("Stopping simulation...");

		StopCoroutine(simulationCoroutine);
		SiteManager.Instance.trafficManager.ResetSimulation();

		uiTabGroup.SelectTab(defaultTabId);

		Debug.Log("Simulation stopped");
		UIManager.Instance.Prompt("Simulation stopped");
	}

	public void OnVehicleSpawnedCallback() {
		simulationStats.NumVehiclesSpawned++;
	}

	public void OnVehicleReachedExitCallback() {
		simulationStats.NumVehiclesReachedExit++;
	}

	public void OnBack() {
		CloseSimulationStats();
	}

	public void OnPublish() {
		/*if (SiteManager.Instance.GenerateAttemptMetadata().ToString() == SessionManager.Instance.AttemptMetadata.ToString()) {
			UIManager.Instance.Prompt("Could not publish attempt without any changes");
			return;
		}*/

		UIManager.Instance.Prompt("Publishing...");
		LevelManager.Instance.Publish(simulationStats.TrafficQuality, simulationStats.Budget, simulationStats.Score);
	}

	private IEnumerator SimulateCoroutine() {
		UIManager.Instance.Prompt("Simulation started");
		uiTabGroup.SelectTab(simulationTabId);

		simulationStats = new SimulationStats();

		// Describes the currently simulated in-game time (in hours, from 0.00f to 24.00f)
		float inGameTime = 0.0f;
		while (inGameTime <= 24.0f) {
			SiteManager.Instance.trafficManager.SimulateAtTime(inGameTime);

			inGameTime += Time.deltaTime * simulationTimescale / 3600.0f;
			simulationStats.TotalSimulationTime += Time.deltaTime;

			if (inGameTime >= 3.0f && inGameTime <= 9.0f) {
				clockDayBackground.color = new Color(
					clockDayBackground.color.r,
					clockDayBackground.color.g,
					clockDayBackground.color.b,
					(inGameTime - 3.0f) / 6.0f
				);

				clockNightBackground.color = new Color(
					clockNightBackground.color.r,
					clockNightBackground.color.g,
					clockNightBackground.color.b,
					(9.0f - inGameTime) / 6.0f
				);
			} else if (inGameTime >= 15.0f && inGameTime <= 21.0f) {
				clockDayBackground.color = new Color(
					clockDayBackground.color.r,
					clockDayBackground.color.g,
					clockDayBackground.color.b,
					(21.0f - inGameTime) / 6.0f
				);

				clockNightBackground.color = new Color(
					clockNightBackground.color.r,
					clockNightBackground.color.g,
					clockNightBackground.color.b,
					(inGameTime - 15.0f) / 6.0f
				);
			}

			clockHand.rotation = Quaternion.Euler(0.0f, 0.0f, inGameTime * -30.0f);
			
			trafficQualitySlider.value = simulationStats.TrafficQuality;
			
			yield return null;
		}

		while (simulationStats.NumVehiclesReachedExit != simulationStats.NumVehiclesSpawned) {
			simulationStats.TotalSimulationTime += Time.deltaTime;

			trafficQualitySlider.value = simulationStats.TrafficQuality;

			yield return null;
		}
		
		uiTabGroup.SelectTab(defaultTabId);
		UIManager.Instance.Prompt("Simulation ended");

		OpenSimulationStats();

		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, SessionManager.Instance.Username);
		postParams.Add(SESSION_ID_PARAM, SessionManager.Instance.SessionId);
		postParams.Add(LEVEL_ID_PARAM, SessionManager.Instance.LevelId.ToString());
		postParams.Add(SCORE_PARAM, simulationStats.Score.ToString());
		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, ATTEMPT_RANK_DIR, 
			postParams, FetchRankCallback));
	}

	private void OpenSimulationStats() {
		simulationOverlay.SetActive(true);

		simulationTime.text = string.Format("{0:0.##} seconds", simulationStats.TotalSimulationTime);
		simulatedVehicles.text = string.Format("{0} vehicles", simulationStats.NumVehiclesSpawned);
		averageSpeed.text = string.Format("{0:0.00} units/s ({1:0.0%})",
			simulationStats.AverageSpeed, simulationStats.TrafficQuality);
		budgetRequired.text = string.Format("$ {0:0.00}", simulationStats.Budget);
		score.text = string.Format("{0:0.000} (Calculating...)", simulationStats.Score);
	}

	private void CloseSimulationStats() {
		simulationOverlay.SetActive(false);
	}

	private void FetchRankCallback(bool success, string response) {
		if (!success) {
			score.text = string.Format("{0:0.000} (?th percentile)", simulationStats.Score);
			return;
		}

		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			score.text = string.Format("{0:0.000} (?th percentile)", simulationStats.Score);
			return;
		}

		SessionManager.Instance.InitializeSession(parsedResponse.username, parsedResponse.session_id);

		ParsedRank parsedRank = JsonUtility.FromJson<ParsedRank>(parsedResponse.data);
		float percentile = 100.0f * (parsedRank.above + 1) / (parsedRank.above + 1 + parsedRank.below);
		
		score.text = string.Format("{0:0.000} ({1:0.#}th percentile)", simulationStats.Score, percentile);
	}

}
