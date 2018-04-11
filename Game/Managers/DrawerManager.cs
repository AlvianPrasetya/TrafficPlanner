using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DrawerManager : MonoBehaviour, IToggleable {

	public Camera sceneCamera;
	public Camera uiCamera;

	public float drawerActivePosition;
	public float drawerActiveWidth;
	public float drawerInactivePosition;
	public float drawerInactiveWidth;
	public float drawerSpeed;

	public Text drawerText;

	public GameObject content;

	private bool active;
	private Coroutine drawerCoroutine;

	public bool IsActive {
		get {
			return active;
		}
	}
	
	public void ToggleActive() {
		active = !active;

		if (active) {
			OnActivated();
		} else {
			OnDeactivated();
		}
	}

	public virtual void OnActivated() {
		drawerText.text = ">";
		content.SetActive(true);

		if (drawerCoroutine != null) {
			StopCoroutine(drawerCoroutine);
		}

		drawerCoroutine = StartCoroutine(ExtendDrawerCoroutine());
	}

	public virtual void OnDeactivated() {
		drawerText.text = "<";
		content.SetActive(false);

		if (drawerCoroutine != null) {
			StopCoroutine(drawerCoroutine);
		}

		drawerCoroutine = StartCoroutine(RetractDrawerCoroutine());
	}

	private void Awake() {
		// Initially drawer needs to be active
		ToggleActive();
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

}
