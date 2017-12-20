using UnityEngine;

public abstract class RoadPreview : MonoBehaviour, IBigridTransform {

	public Transform head;
	public Transform body;
	public Transform tail;
	public Renderer headRenderer;
	public Renderer bodyRenderer;
	public Renderer tailRenderer;
	public Material validBodyMaterial;
	public Material validEndMaterial;
	public Material invalidBodyMaterial;
	public Material invalidEndMaterial;
	
	private Grid startGrid;
	private Grid endGrid;
	private float roadLength;
	private bool valid;

	public Grid StartGrid {
		get {
			return startGrid;
		}

		set {
			startGrid = value;

			if (startGrid != null && endGrid != null) {
				Vector3 roadVector = endGrid.Coordinates - startGrid.Coordinates;
				Vector3 forwardDirection = (roadVector == Vector3.zero) ? Vector3.forward : roadVector.normalized;
				transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
				roadLength = roadVector.magnitude;
				head.position = tail.position - forwardDirection * roadLength;
				body.localScale = new Vector3(
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					roadLength);
				bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);

				IsValid = Validate();
			} else {
				IsValid = false;
			}
		}
	}

	public Grid EndGrid {
		get {
			return endGrid;
		}

		set {
			endGrid = value;

			if (startGrid != null && endGrid != null) {
				Vector3 roadVector = endGrid.Coordinates - startGrid.Coordinates;
				Vector3 forwardDirection = (roadVector == Vector3.zero) ? Vector3.forward : roadVector.normalized;
				transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
				roadLength = roadVector.magnitude;
				body.localScale = new Vector3(
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					roadLength);
				bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
				tail.position = head.position + forwardDirection * roadLength;

				IsValid = Validate();
			} else {
				IsValid = false;
			}
		}
	}

	public void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail) {
		head = null;
		tail = null;
		return;
	}

	public bool IsValid {
		get {
			return valid;
		}

		private set {
			valid = value;
			bodyRenderer.material = (valid) ? validBodyMaterial : invalidBodyMaterial;
			bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
			headRenderer.material = tailRenderer.material = (valid) ? validEndMaterial : invalidEndMaterial;
		}
	}

	private bool Validate() {
		if (startGrid == null || endGrid == null) {
			return false;
		}

		if (startGrid == endGrid) {
			return false;
		}

		// Check for intersection with occupied grids
		Vector3 rayVector = endGrid.transform.position - startGrid.transform.position;
		RaycastHit hitInfo;
		if (Physics.SphereCast(
			startGrid.transform.position + Vector3.up * startGrid.transform.localScale.y * 0.5f,
			startGrid.transform.localScale.y * 0.392f, // The min sphere radius to disallow < 20% slope
			rayVector.normalized,
			out hitInfo,
			rayVector.magnitude,
			LayerUtils.Mask.OCCUPIED_GRID)) {
			return false;
		}

		return true;
	}

}
