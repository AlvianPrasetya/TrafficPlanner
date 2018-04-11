using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	public Text promptText;

	public GameObject onMousePrompt;
	public Text onMousePromptText;

	public float promptFadeTime;

	private static UIManager instance;

	private Coroutine activeFadePromptCoroutine;
	
	public static UIManager Instance {
		get {
			return instance;
		}
	}

	public void Prompt(string promptMessage) {
		promptText.text = promptMessage;

		if (activeFadePromptCoroutine != null) {
			StopCoroutine(activeFadePromptCoroutine);
		}
		activeFadePromptCoroutine = StartCoroutine(FadePrompt());
	}

	public void ShowOnMousePrompt(string promptMessage) {
		onMousePrompt.SetActive(true);
		onMousePromptText.text = promptMessage;
	}

	public void HideOnMousePrompt() {
		onMousePromptText.text = "";
		onMousePrompt.SetActive(false);
	}

	private void Awake() {
		instance = this;
	}

	private IEnumerator FadePrompt() {
		float timeSincePrompt = 0.0f;
		while (timeSincePrompt < promptFadeTime) {
			promptText.color = new Color(
				promptText.color.r, 
				promptText.color.g, 
				promptText.color.b, 
				1.0f - timeSincePrompt / promptFadeTime);

			timeSincePrompt += Time.deltaTime;

			yield return null;
		}

		activeFadePromptCoroutine = null;
	}

}
