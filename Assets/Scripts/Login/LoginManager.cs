using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	private static readonly string LOGIN_DIR = "/login.php";
	private static readonly string REGISTER_DIR = "/register.php";


	private static readonly string USERNAME_EMPTY_PROMPT = "Please enter your username";
	private static readonly string PASSWORD_EMPTY_PROMPT = "Please enter your password";
	private static readonly string LOGGING_IN_PROMPT = "Logging in...";
	private static readonly string LOG_IN_SUCCESS_PROMPT = "Successfully logged in";
	private static readonly string LOG_IN_FAILURE_PROMPT = "Invalid username and password combination";
	private static readonly string REGISTERING_PROMPT = "Registering...";
	private static readonly string REGISTER_SUCCESS_PROMPT = "Successfully registered. You can now log in";
	private static readonly string REGISTER_FAILURE_PROMPT = "That username already exists";

	private static readonly string USERNAME_PARAM = "username";
	private static readonly string PASSWORD_PARAM = "password";

	public Text prompt;
	public InputField usernameField;
	public InputField passwordField;

	public void OnLoginClick() {
		if (usernameField.text == "") {
			Prompt(USERNAME_EMPTY_PROMPT);
			FocusUsername();
			return;
		}

		if (passwordField.text == "") {
			Prompt(PASSWORD_EMPTY_PROMPT);
			FocusPassword();
			return;
		}

		Login(usernameField.text, passwordField.text);
	}

	public void OnRegisterClick() {
		if (usernameField.text == "") {
			Prompt(USERNAME_EMPTY_PROMPT);
			FocusUsername();
			return;
		}

		if (passwordField.text == "") {
			Prompt(PASSWORD_EMPTY_PROMPT);
			FocusPassword();
			return;
		}

		Register(usernameField.text, passwordField.text);
	}

	public void OnForgotPasswordClick() {

	}

	private void Start() {
		FocusUsername();
	}

	private void Update() {
		InputNextField();
		InputLogin();
	}

	private void FocusUsername() {
		usernameField.Select();
		usernameField.ActivateInputField();
	}

	private void FocusPassword() {
		passwordField.Select();
		passwordField.ActivateInputField();
	}

	private void Prompt(string promptText) {
		prompt.text = promptText;
	}

	private void InputNextField() {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			if (usernameField.isFocused) {
				FocusPassword();
			} else if (passwordField.isFocused) {
				FocusUsername();
			}
		}
	}

	private void InputLogin() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			OnLoginClick();
		}
	}

	private void Login(string username, string password) {
		Prompt(LOGGING_IN_PROMPT);

		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, username);
		postParams.Add(PASSWORD_PARAM, password);
		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, LOGIN_DIR, postParams, LoginCallback));
	}

	private void LoginCallback(bool success, string response) {
		if (!success) {
			Prompt(LOG_IN_FAILURE_PROMPT);
			FocusUsername();
			return;
		}
		
		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			Prompt(LOG_IN_FAILURE_PROMPT);
			FocusUsername();
			return;
		}

		Prompt(LOG_IN_SUCCESS_PROMPT);
		SessionManager.Instance.InitializeSession(parsedResponse.username, parsedResponse.session_id);

		SceneManager.LoadScene(SceneUtils.LOBBY);
	}

	private void Register(string username, string password) {
		Prompt(REGISTERING_PROMPT);

		Dictionary<string, string> postParams = new Dictionary<string, string>();
		postParams.Add(USERNAME_PARAM, username);
		postParams.Add(PASSWORD_PARAM, password);
		StartCoroutine(Auth.AsyncPost(
			SessionManager.Instance.host, REGISTER_DIR, postParams, RegisterCallback));
	}

	private void RegisterCallback(bool success, string response) {
		if (!success) {
			Prompt(REGISTER_FAILURE_PROMPT);
			FocusUsername();
			return;
		}

		ParsedResponse parsedResponse = JsonUtility.FromJson<ParsedResponse>(response);

		if (!parsedResponse.success) {
			Prompt(REGISTER_FAILURE_PROMPT);
			FocusUsername();
			return;
		}

		Prompt(REGISTER_SUCCESS_PROMPT);
	}

}
