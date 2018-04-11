using UnityEngine;
using System.Collections.Generic;

public abstract class Road : MonoBehaviour, IBigridTransform, IDemolishable {

	public bool selectable;
	public bool demolishable;

	protected Grid startGrid;
	protected Grid endGrid;

	public Transform head;
	public Transform body;
	public Transform tail;

	public Renderer headRenderer;
	public Renderer bodyRenderer;
	public Renderer tailRenderer;

	public Material defaultBodyMaterial;
	public Material defaultEndMaterial;
	public Material selectedBodyMaterial;
	public Material selectedEndMaterial;
	public Material demolishBodyMaterial;
	public Material demolishEndMaterial;

	protected Vector3 roadDirection;
	protected float roadLength;
	private HashSet<Road> incomingRoads;
	private HashSet<Road> outgoingRoads;

	public virtual Grid StartGrid {
		get {
			return startGrid;
		}

		set {
			startGrid = value;
			incomingRoads.Clear();

			if (startGrid != null && endGrid != null) {
				Vector3 roadVector = endGrid.Coordinates - startGrid.Coordinates;
				roadDirection = (roadVector == Vector3.zero) ? Vector3.forward : roadVector.normalized;
				roadLength = roadVector.magnitude;
				transform.rotation = Quaternion.LookRotation(roadDirection, Vector3.up);
				head.position = tail.position - roadDirection * roadLength;
				body.localScale = new Vector3(
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					roadLength);
				bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
			} else {
				roadDirection = Vector3.zero;
				roadLength = 0.0f;
			}
		}
	}

	public virtual Grid EndGrid {
		get {
			return endGrid;
		}

		set {
			endGrid = value;
			outgoingRoads.Clear();
			
			if (startGrid != null && endGrid != null) {
				Vector3 roadVector = endGrid.Coordinates - startGrid.Coordinates;
				roadDirection = (roadVector == Vector3.zero) ? Vector3.forward : roadVector.normalized;
				roadLength = roadVector.magnitude;
				transform.rotation = Quaternion.LookRotation(roadDirection, Vector3.up);
				body.localScale = new Vector3(
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					(roadLength == 0.0f) ? 0.0f : 1.0f,
					roadLength);
				bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
				tail.position = head.position + roadDirection * roadLength;
			} else {
				roadDirection = Vector3.zero;
				roadLength = 0.0f;
			}
		}
	}

	public void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail) {
		if (intermediateGrid == startGrid) {
			// Intermediate grid is the start grid, no need to split the road, return this road as the tail
			head = null;
			tail = this;
			return;
		}

		if (intermediateGrid == endGrid) {
			// Intermediate grid is the end grid, no need to split the road, return this road as the head
			head = this;
			tail = null;
			return;
		}

		tail = Instantiate(this, intermediateGrid.transform.position, intermediateGrid.transform.rotation, 
			SiteManager.Instance.roadManager.transform);
		tail.StartGrid = intermediateGrid;
		tail.EndGrid = endGrid;

		// Copy old outgoing roads from head to tail
		foreach (Road outgoingRoad in outgoingRoads) {
			((Road) tail).AddOutgoingRoad(outgoingRoad);
		}

		head = this;
		head.EndGrid = intermediateGrid;
		
		// Establish links between the new tail and head
		((Road) tail).AddIncomingRoad((Road) head);
		((Road) head).AddOutgoingRoad((Road) tail);
		return;
	}

	public bool IsSelectable {
		get {
			return selectable;
		}
	}

	public void Select() {
		headRenderer.material = tailRenderer.material = selectedEndMaterial;
		bodyRenderer.material = selectedBodyMaterial;
		bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
	}

	public void Unselect() {
		headRenderer.material = tailRenderer.material = defaultEndMaterial;
		bodyRenderer.material = defaultBodyMaterial;
		bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
	}
	
	public bool IsDemolishable {
		get {
			return demolishable;
		}
	}

	public void IndicateDemolish() {
		headRenderer.material = tailRenderer.material = demolishEndMaterial;
		bodyRenderer.material = demolishBodyMaterial;
		bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
	}

	public void UnindicateDemolish() {
		headRenderer.material = tailRenderer.material = defaultEndMaterial;
		bodyRenderer.material = defaultBodyMaterial;
		bodyRenderer.material.mainTextureScale = new Vector2(bodyRenderer.material.mainTextureScale.x, roadLength);
	}

	public virtual void Demolish() {
		foreach (Road incomingRoad in incomingRoads) {
			incomingRoad.RemoveOutgoingRoad(this);
		}

		foreach (Road outgoingRoad in outgoingRoads) {
			outgoingRoad.RemoveIncomingRoad(this);
		}

		Destroy(gameObject);
	}

	public Vector3 RoadDirection {
		get {
			return roadDirection;
		}
	}

	public float RoadLength {
		get {
			return roadLength;
		}
	}

	public IEnumerable<Road> IncomingRoads {
		get {
			return incomingRoads;
		}
	}

	public void AddIncomingRoad(Road incomingRoad) {
		if (!incomingRoads.Contains(incomingRoad)) {
			incomingRoads.Add(incomingRoad);
		}
	}

	public void RemoveIncomingRoad(Road incomingRoad) {
		if (incomingRoads.Contains(incomingRoad)) {
			incomingRoads.Remove(incomingRoad);
		}
	}

	public IEnumerable<Road> OutgoingRoads {
		get {
			return outgoingRoads;
		}
	}

	public void AddOutgoingRoad(Road outgoingRoad) {
		if (!outgoingRoads.Contains(outgoingRoad)) {
			outgoingRoads.Add(outgoingRoad);
		}
	}

	public void RemoveOutgoingRoad(Road outgoingRoad) {
		if (outgoingRoads.Contains(outgoingRoad)) {
			outgoingRoads.Remove(outgoingRoad);
		}
	}

	protected virtual void Awake() {
		incomingRoads = new HashSet<Road>();
		outgoingRoads = new HashSet<Road>();
	}

}
