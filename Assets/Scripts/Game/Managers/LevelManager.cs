using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	private static readonly string CREATE_ATTEMPT_DIR = "/create_attempt.php";
	private static readonly string PUBLISH_ATTEMPT_SUCCESS_PROMPT = "Attempt has been successfully published";
	private static readonly string PUBLISH_ATTEMPT_FAILURE_PROMPT = "Attempt could not be published due to server errors";

	private static readonly string USERNAME_PARAM = "username";
	private static readonly string SESSION_ID_PARAM = "session_id";
	private static readonly string LEVEL_ID_PARAM = "level_id";
	private static readonly string ATTEMPT_METADATA_PARAM = "attempt_metadata";
	private static readonly string AVERAGE_SPEED_PARAM = "avg_speed";
	private static readonly string BUDGET_REQUIRED_PARAM = "budget_req";
	private static readonly string SCORE_PARAM = "score";

	private static LevelManager instance;

	public static LevelManager Instance {
		get {
			return instance;
		}
	}

	public void Publish(float avgSpeed, float budgetReq, float score) {
		// Prepare GET params
		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, SessionManager.Instance.Username);
		postParams.Add(SESSION_ID_PARAM, SessionManager.Instance.SessionId);
		postParams.Add(LEVEL_ID_PARAM, SessionManager.Instance.LevelId.ToString());
		postParams.Add(ATTEMPT_METADATA_PARAM, SiteManager.Instance.GenerateAttemptMetadata().ToString());
		postParams.Add(AVERAGE_SPEED_PARAM, avgSpeed.ToString());
		postParams.Add(BUDGET_REQUIRED_PARAM, budgetReq.ToString());
		postParams.Add(SCORE_PARAM, score.ToString());

		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, CREATE_ATTEMPT_DIR,
			postParams, PublishCallback));
	}

	public void OnQuit() {
		SceneManager.LoadScene(SceneUtils.LOBBY);
	}

	private void Awake() {
		instance = this;
	}

	private void Start() {
		StartCoroutine(SiteManager.Instance.GenerateSite(
			SessionManager.Instance.LevelMetadata, 
			SessionManager.Instance.AttemptMetadata));
	}

	private void PublishCallback(bool success, string response) {
		if (!success) {
			UIManager.Instance.Prompt(PUBLISH_ATTEMPT_FAILURE_PROMPT);
			return;
		}

		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			UIManager.Instance.Prompt(PUBLISH_ATTEMPT_FAILURE_PROMPT);
			return;
		}

		UIManager.Instance.Prompt(PUBLISH_ATTEMPT_SUCCESS_PROMPT);
		SessionManager.Instance.InitializeSession(parsedResponse.username, parsedResponse.session_id);
	}

}
