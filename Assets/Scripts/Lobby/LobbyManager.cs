using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour {

	private static readonly string LEVELS_DIR = "/level_list.php";
	private static readonly string FETCHING_LEVELS_PROMPT = "Fetching levels...";
	private static readonly string FETCH_LEVELS_SUCCESS_PROMPT = "Successfully fetched levels";
	private static readonly string FETCH_LEVELS_FAILURE_PROMPT = "Failed to fetch levels";

	private static readonly string ATTEMPTS_DIR = "/attempt_list.php";
	private static readonly string FETCHING_ATTEMPTS_PROMPT = "Fetching attempts...";
	private static readonly string FETCH_ATTEMPTS_SUCCESS_PROMPT = "Successfully fetched attempts";
	private static readonly string FETCH_ATTEMPTS_FAILURE_PROMPT = "Failed to fetch attempts";

	private static readonly string USERNAME_PARAM = "username";
	private static readonly string SESSION_ID_PARAM = "session_id";
	private static readonly string LEVEL_ID_PARAM = "level_id";

	public LevelListManager levelListManager;

	public Text prompt;

	private List<LevelEntry> displayedLevels;

	private static LobbyManager instance;

	public static LobbyManager Instance {
		get {
			return instance;
		}
	}

	public void OnDesignLevelClick() {
		DesignLevel();
	}

	public void OnRefreshClick() {
		FetchLevels();
	}

	public void FetchAttempts(int levelId) {
		Prompt(FETCHING_ATTEMPTS_PROMPT);
		
		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, SessionManager.Instance.Username);
		postParams.Add(SESSION_ID_PARAM, SessionManager.Instance.SessionId);
		postParams.Add(LEVEL_ID_PARAM, levelId.ToString());

		// Request for attempt list from the server
		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, ATTEMPTS_DIR,
			postParams, FetchAttemptsCallback));
	}

	private void Awake() {
		instance = this;
	}

	private void Start() {
		FetchLevels();
	}

	private void Prompt(string promptText) {
		prompt.text = promptText;
	}

	private void DesignLevel() {
		SceneManager.LoadScene(SceneUtils.LEVEL_DESIGN);
	}

	private void FetchLevels() {
		Prompt(FETCHING_LEVELS_PROMPT);

		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, SessionManager.Instance.Username);
		postParams.Add(SESSION_ID_PARAM, SessionManager.Instance.SessionId);

		// Request for level list from the server
		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, LEVELS_DIR,
			postParams, FetchLevelsCallback));
	}

	private void FetchLevelsCallback(bool success, string response) {
		if (!success) {
			Prompt(FETCH_LEVELS_FAILURE_PROMPT);
			return;
		}

		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			Prompt(FETCH_LEVELS_FAILURE_PROMPT);
			return;
		}

		Prompt(FETCH_LEVELS_SUCCESS_PROMPT);
		SessionManager.Instance.InitializeSession(parsedResponse.username, parsedResponse.session_id);

		ParsedLevelCollection parsedLevelCollection = JsonUtility.FromJson<ParsedLevelCollection>(parsedResponse.data);

		levelListManager.ClearLevels();
		foreach (ParsedLevel parsedLevel in parsedLevelCollection.levels) {
			levelListManager.AddLevelEntry(parsedLevel.level_id, parsedLevel.level_name, parsedLevel.level_designer,
				parsedLevel.level_metadata);
		}
	}

	private void FetchAttemptsCallback(bool success, string response) {
		if (!success) {
			Prompt(FETCH_ATTEMPTS_FAILURE_PROMPT);
			return;
		}

		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			Prompt(FETCH_ATTEMPTS_FAILURE_PROMPT);
			return;
		}

		Prompt(FETCH_ATTEMPTS_SUCCESS_PROMPT);
		SessionManager.Instance.InitializeSession(parsedResponse.username, parsedResponse.session_id);

		ParsedAttemptCollection parsedAttemptCollection = JsonUtility.FromJson<ParsedAttemptCollection>(parsedResponse.data);

		levelListManager.ClearAttempts();
		foreach (ParsedAttempt parsedAttempt in parsedAttemptCollection.attempts) {
			levelListManager.AddAttemptEntry(
				parsedAttempt.level_id, parsedAttempt.level_name, parsedAttempt.attempt_designer,
				parsedAttempt.level_metadata, parsedAttempt.attempt_metadata, 
				parsedAttempt.avg_speed, parsedAttempt.budget_req, parsedAttempt.score);
		}

		levelListManager.OpenOverlay();
	}

}
