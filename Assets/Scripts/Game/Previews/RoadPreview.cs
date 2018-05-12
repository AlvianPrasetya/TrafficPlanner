using UnityEngine;

public class RoadPreview : MonoBehaviour, IBigridTransform, IValidatable {

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
	private string invalidStatus;

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
			}

			Validate();
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
			}

			Validate();
		}
	}

	public void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail) {
		head = null;
		tail = null;
		return;
	}

	public float RoadLength {
		get {
			return roadLength;
		}
	}

	public bool IsValid {
		get {
			return valid;
		}

		set {
			valid = value;
			invalidStatus = valid ? "" : invalidStatus;
			bodyRenderer.material = valid ? validBodyMaterial : invalidBodyMaterial;
			bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
			headRenderer.material = tailRenderer.material = valid ? validEndMaterial : invalidEndMaterial;
		}
	}

	public string InvalidStatus {
		get {
			return invalidStatus;
		}
	}

	public void Validate() {
		if (startGrid == null || endGrid == null) {
			IsValid = false;
			invalidStatus = "No end grid selected";
			return;
		}

		if (startGrid == endGrid) {
			IsValid = false;
			invalidStatus = "Same start and end grids";
			return;
		}

		if (CalculateSqrSlope() > 0.041f) {
			IsValid = false;
			invalidStatus = "Road too steep!";
			return;
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
			IsValid = false;
			invalidStatus = "Road obstructed";
			return;
		}

		IsValid = true;
	}

	private float CalculateSqrSlope() {
		Vector2 flattenedRoadVector = new Vector2(
			endGrid.Coordinates.x - startGrid.Coordinates.x, 
			endGrid.Coordinates.z - startGrid.Coordinates.z);

		float sqrLength = Vector2.SqrMagnitude(flattenedRoadVector);
		float sqrHeight = Mathf.Pow(endGrid.Coordinates.y - startGrid.Coordinates.y, 2);

		return sqrHeight / sqrLength;
	}

}
