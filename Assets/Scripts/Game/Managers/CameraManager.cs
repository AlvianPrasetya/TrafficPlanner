using UnityEngine;

public class CameraManager : MonoBehaviour {

	public Camera sceneCamera;

	public Vector3 initialPosition;
	public Vector3 initialRotation;
	public float initialFieldOfView;
	
	public float baseTranslateSpeed;
	public float baseRotateSpeed;
	public float baseZoomSpeed;

	private bool transforming;
	private bool translating;
	private bool rotating;
	private Vector2 previousMousePosition;

	public void ResetCamera() {
		sceneCamera.transform.position = initialPosition;
		sceneCamera.transform.rotation = Quaternion.Euler(initialRotation);
		sceneCamera.fieldOfView = initialFieldOfView;
	}

	private void Awake() {
		transforming = false;
		translating = false;
		rotating = false;
	}

	private void Update() {
		TransformCameraRoutines();
	}

	private void TransformCameraRoutines() {
		InputBeginTransformCamera();
		InputTransformCamera();
		InputEndTransformCamera();
	}

	private void InputBeginTransformCamera() {
		if (Input.GetKeyDown(KeyCode.LeftAlt)) {
			transforming = true;
		}
	}

	private void InputTransformCamera() {
		if (transforming) {
			TranslateCameraRoutines();
			RotateCameraRoutines();
			InputZoomCamera();
		}
	}

	private void InputEndTransformCamera() {
		if (Input.GetKeyUp(KeyCode.LeftAlt)) {
			transforming = false;
			translating = false;
			rotating = false;
		}
	}

	private void TranslateCameraRoutines() {
		InputBeginTranslateCamera();
		InputTranslateCamera();
		InputEndTranslateCamera();
	}

	private void InputBeginTranslateCamera() {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			translating = true;
			previousMousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		}
	}

	private void InputTranslateCamera() {
		if (!translating) {
			return;
		}

		Vector2 mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

		sceneCamera.transform.Translate(
			Vector3.left * (mousePosition.x - previousMousePosition.x) * baseTranslateSpeed * Time.deltaTime,
			Space.Self);

		sceneCamera.transform.Translate(
			Vector3.down * (mousePosition.y - previousMousePosition.y) * baseTranslateSpeed * Time.deltaTime,
			Space.Self);

		previousMousePosition = mousePosition;
	}

	private void InputEndTranslateCamera() {
		if (Input.GetMouseButtonUp(InputUtils.MOUSE_BUTTON_LEFT)) {
			translating = false;
		}
	}

	private void InputZoomCamera() {
		float scrollValue = Input.GetAxis("Mouse ScrollWheel");
		if (scrollValue < 0.0f) {
			sceneCamera.transform.Translate(-Vector3.forward * baseZoomSpeed * Time.deltaTime, Space.Self);
		} else if (scrollValue > 0.0f) {
			sceneCamera.transform.Translate(Vector3.forward * baseZoomSpeed * Time.deltaTime, Space.Self);
		}
	}

	private void RotateCameraRoutines() {
		InputBeginRotateCamera();
		InputRotateCamera();
		InputEndRotateCamera();
	}

	private void InputBeginRotateCamera() {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_RIGHT)) {
			rotating = true;
			previousMousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		}
	}

	private void InputRotateCamera() {
		if (!rotating) {
			return;
		}

		Vector2 mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

		sceneCamera.transform.RotateAround(
			Vector3.zero, 
			Vector3.up, 
			(mousePosition.x - previousMousePosition.x) * baseRotateSpeed * Time.deltaTime);

		sceneCamera.transform.RotateAround(
			Vector3.zero, 
			-sceneCamera.transform.right, 
			(mousePosition.y - previousMousePosition.y) * baseRotateSpeed * Time.deltaTime);

		previousMousePosition = mousePosition;
	}

	private void InputEndRotateCamera() {
		if (Input.GetMouseButtonUp(InputUtils.MOUSE_BUTTON_RIGHT)) {
			rotating = false;
		}
	}

}
