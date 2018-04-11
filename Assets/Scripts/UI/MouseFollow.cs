using UnityEngine;

public class MouseFollow : MonoBehaviour {

	public Camera referenceCamera;
	public RectTransform referenceRectTransform;
	public RectTransform targetRectTransform;

	private void Update() {
		Vector2 renderDimensions = new Vector2(
			Screen.width * referenceCamera.rect.width, Screen.height * referenceCamera.rect.height);

		Vector2 relativeMousePosition = new Vector2(
			Input.mousePosition.x / renderDimensions.x, Input.mousePosition.y / renderDimensions.y);

		Vector2 targetPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			referenceRectTransform, Input.mousePosition, referenceCamera, out targetPosition);

		targetRectTransform.anchoredPosition = targetPosition;
	}

}
