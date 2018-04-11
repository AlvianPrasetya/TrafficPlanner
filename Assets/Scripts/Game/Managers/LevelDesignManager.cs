using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDesignManager : MonoBehaviour {

	private static readonly string CREATE_LEVEL_DIR = "/create_level.php";
	private static readonly string PUBLISH_LEVEL_SUCCESS_PROMPT = "Level has been successfully published";
	private static readonly string PUBLISH_LEVEL_FAILURE_PROMPT = "Level could not be published due to server errors";

	private static readonly string USERNAME_PARAM = "username";
	private static readonly string SESSION_ID_PARAM = "session_id";
	private static readonly string LEVEL_NAME_PARAM = "level_name";
	private static readonly string LEVEL_METADATA_PARAM = "level_metadata";

	public InputField levelNameField;
	public InputField xDimensionsField;
	public InputField yDimensionsField;
	public InputField zDimensionsField;

	private static LevelDesignManager instance;

	public static LevelDesignManager Instance {
		get {
			return instance;
		}
	}

	public void OnRebuild() {
		int xDimension, yDimension, zDimension;
		try {
			xDimension = int.Parse(xDimensionsField.text);
			yDimension = int.Parse(yDimensionsField.text);
			zDimension = int.Parse(zDimensionsField.text);
		} catch (Exception) {
			// Invalid dimensions format
			return;
		}

		StartCoroutine(SiteManager.Instance.GenerateSite(xDimension, yDimension, zDimension));
	}

	public void OnPublish() {
		// Prepare GET params
		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, SessionManager.Instance.Username);
		postParams.Add(SESSION_ID_PARAM, SessionManager.Instance.SessionId);
		postParams.Add(LEVEL_NAME_PARAM, levelNameField.text);
		postParams.Add(LEVEL_METADATA_PARAM, SiteManager.Instance.GenerateLevelMetadata().ToString());
		
		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, CREATE_LEVEL_DIR,
			postParams, PublishCallback));
	}

	public void OnQuit() {
		SceneManager.LoadScene(SceneUtils.LOBBY);
	}

	private void Awake() {
		instance = this;
	}

	private void FocusLevelName() {
		levelNameField.Select();
		levelNameField.ActivateInputField();
	}

	private void PublishCallback(bool success, string response) {
		if (!success) {
			UIManager.Instance.Prompt(PUBLISH_LEVEL_FAILURE_PROMPT);
			FocusLevelName();
			return;
		}

		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			UIManager.Instance.Prompt(PUBLISH_LEVEL_FAILURE_PROMPT);
			FocusLevelName();
			return;
		}

		UIManager.Instance.Prompt(PUBLISH_LEVEL_SUCCESS_PROMPT);
		SessionManager.Instance.InitializeSession(parsedResponse.username, parsedResponse.session_id);
	}

}
