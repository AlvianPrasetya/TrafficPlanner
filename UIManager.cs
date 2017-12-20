using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Camera sceneCamera;
	public Camera uiCamera;

	public float drawerActivePosition;
	public float drawerActiveWidth;
	public float drawerInactivePosition;
	public float drawerInactiveWidth;
	public float drawerSpeed;

	public GameObject content;

	public Text promptText;
	public float promptFadeTime;

	public GameObject overlayPanel;
	public Text overlayText;
	public Button overlayLeftButton;
	public Text overlayLeftButtonText;
	public Button overlayRightButton;
	public Text overlayRightButtonText;

	public Text drawerText;

	private static UIManager instance;

	private Coroutine activeFadePromptCoroutine;

	private bool drawerActive;
	private Coroutine drawerCoroutine;

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

	public void OpenOverlay(string overlayMessage,
		UnityAction leftButtonAction, string leftButtonMessage,
		UnityAction rightButtonAction, string rightButtonMessage) {
		overlayPanel.SetActive(true);
		overlayText.text = overlayMessage;

		overlayLeftButton.onClick.RemoveAllListeners();
		overlayLeftButton.onClick.AddListener(leftButtonAction);
		overlayLeftButtonText.text = leftButtonMessage;

		overlayRightButton.onClick.RemoveAllListeners();
		overlayRightButton.onClick.AddListener(rightButtonAction);
		overlayRightButtonText.text = rightButtonMessage;
	}

	public void CloseOverlay() {
		overlayPanel.SetActive(false);
	}

	public void ToggleDrawer() {
		drawerActive = !drawerActive;
		drawerText.text = (drawerActive) ? ">" : "<";
		content.SetActive(drawerActive);

		if (drawerCoroutine != null) {
			StopCoroutine(drawerCoroutine);
		}

		drawerCoroutine = (drawerActive) ? StartCoroutine(ExtendDrawerCoroutine()) : StartCoroutine(RetractDrawerCoroutine());
	}

	private void Awake() {
		instance = this;
		drawerActive = true;
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

	private IEnumerator RetractDrawerCoroutine() {
		while (uiCamera.rect.position.x < drawerInactivePosition || uiCamera.rect.size.x > drawerInactiveWidth) {
			uiCamera.rect = new Rect(
				new Vector2(
					Mathf.Clamp(uiCamera.rect.position.x + drawerSpeed * Time.deltaTime, drawerActivePosition, drawerInactivePosition),
					0.0f), 
				new Vector2(
					Mathf.Clamp(uiCamera.rect.size.x - drawerSpeed * Time.deltaTime, drawerInactiveWidth, drawerActiveWidth), 
					1.0f));

			sceneCamera.rect = new Rect(
				Vector2.zero, 
				new Vector2(
					Mathf.Clamp(
						sceneCamera.rect.size.x + drawerSpeed * Time.deltaTime,
						1.0f - drawerActiveWidth,
						1.0f - drawerInactiveWidth),
					1.0f));

			yield return null;
		}

		drawerCoroutine = null;
	}

	private IEnumerator ExtendDrawerCoroutine() {
		while (uiCamera.rect.position.x > drawerActivePosition || uiCamera.rect.size.x < drawerActiveWidth) {
			uiCamera.rect = new Rect(
				new Vector2(
					Mathf.Clamp(uiCamera.rect.position.x - drawerSpeed * Time.deltaTime, drawerActivePosition, drawerInactivePosition),
					0.0f),
				new Vector2(
					Mathf.Clamp(uiCamera.rect.size.x + drawerSpeed * Time.deltaTime, drawerInactiveWidth, drawerActiveWidth),
					1.0f));

			sceneCamera.rect = new Rect(
				Vector2.zero,
				new Vector2(
					Mathf.Clamp(
						sceneCamera.rect.size.x - drawerSpeed * Time.deltaTime, 
						1.0f - drawerActiveWidth, 
						1.0f - drawerInactiveWidth),
					1.0f));

			yield return null;
		}

		drawerCoroutine = null;
	}

}
