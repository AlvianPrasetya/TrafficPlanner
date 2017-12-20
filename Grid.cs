using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour, ISelectable, ICollectible {

	public enum GridState {
		EMPTY, 
		SELECTABLE, 
		OCCUPIED, 
		SELECTED
	}
	
	public bool selectable;

	public new Renderer renderer;

	public Material emptyMaterial;
	public Material selectableMaterial;
	public Material occupiedMaterial;
	public Material selectedMaterial;

	private GridState state;
	private Vector3 coordinates;
	private Dictionary<Vector3, Grid> neighbours;

	public bool IsSelectable {
		get {
			return selectable;
		}
	}

	public void Select() {
		State = GridState.SELECTED;
	}

	public void Unselect() {
		State = GridState.SELECTABLE;
	}

	public void Enlist() {
		SiteManager.Instance.gridManager.Grids[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z] = this;
	}

	public void Delist() {
		SiteManager.Instance.gridManager.Grids[(int) coordinates.x, (int) coordinates.y, (int) coordinates.z] = null;
	}

	public GridState State {
		get {
			return state;
		}

		set {
			state = value;
			switch (state) {
				case GridState.EMPTY:
					renderer.material = emptyMaterial;
					gameObject.layer = LayerUtils.Id.EMPTY_GRID;
					break;
				case GridState.SELECTABLE:
					renderer.material = selectableMaterial;
					gameObject.layer = LayerUtils.Id.SELECTABLE_GRID;
					break;
				case GridState.OCCUPIED:
					renderer.material = occupiedMaterial;
					gameObject.layer = LayerUtils.Id.OCCUPIED_GRID;
					break;
				case GridState.SELECTED:
					renderer.material = selectedMaterial;
					break;
				default:
					break;
			}
		}
	}

	public Vector3 Coordinates {
		get {
			return coordinates;
		}

		set {
			coordinates = value;
			Enlist();
		}
	}

	public Grid GetNeighbour(Vector3 neighbourDirection) {
		Grid neighbourGrid;
		if (neighbours.TryGetValue(neighbourDirection, out neighbourGrid)) {
			return neighbourGrid;
		} else {
			return null;
		}
	}

	public void SetNeighbour(Vector3 neighbourDirection, Grid neighbourGrid) {
		neighbours[neighbourDirection] = neighbourGrid;
	}

	public bool IsHidden {
		set {
			renderer.enabled = !value;
		}
	}

	private void Awake() {
		neighbours = new Dictionary<Vector3, Grid>();
	}

	private void OnDestroy() {
		Delist();
	}

	/*public Material defaultMaterial;
	public Material persistentMaterial;
	public Material topOfStackMaterial;
	public Material entryPointMaterial;
	public Material exitPointMaterial;
	public Material grassMaterial;
	public Material highlightedMaterial;

	public Material leftwardMaterial;
	public Material rightwardMaterial;
	public Material forwardMaterial;
	public Material backwardMaterial;

	public Material forwardAndLeftwardMaterial;
	public Material forwardAndRightwardMaterial;
	public Material backwardAndLeftwardMaterial;
	public Material backwardAndRightwardMaterial;

	public float defaultHeight;
	public float grassHeight;
	public float roadHeight;

	protected new Renderer renderer;
	
	private Material unhighlightedMaterial;

	private Vector3 coordinates;
	private Dictionary<Vector3, Grid> neighbours;

	private bool persistent;
	private bool topOfStack;
	public bool occupied;
	private bool entryPoint;
	private bool exitPoint;
	private bool highlighted;

	// Mutually exclusive grid properties
	private bool grass;
	private int roadHorizontalDirection;
	private int roadVerticalDirection;

	public Vector3 Coordinates {
		get {
			return coordinates;
		}

		set {
			coordinates = value;
		}
	}

	public void SetNeighbour(Vector3 neighbourDirection, Grid neighbourGrid) {
		neighbours[neighbourDirection] = neighbourGrid;
	}

	public Grid GetNeighbour(Vector3 neighbourDirection) {
		Grid neighbourGrid;
		if (neighbours.TryGetValue(neighbourDirection, out neighbourGrid)) {
			return neighbourGrid;
		} else {
			return null;
		}
	}

	public bool IsPersistent {
		get {
			return persistent;
		}

		set {
			persistent = value;
			if (persistent) {
				renderer.material = unhighlightedMaterial = persistentMaterial;
				if (IsTopOfStack) {
					IsTopOfStack = false;
					neighbours[Vector3.up].IsTopOfStack = true;
				}
			}
		}
	}

	public bool IsTopOfStack {
		get {
			return topOfStack;
		}

		set {
			topOfStack = value;
			gameObject.layer = (topOfStack) ? LayerUtils.Id.TOP_GRID : LayerUtils.Id.GRID;
			renderer.material = unhighlightedMaterial = (topOfStack) ? topOfStackMaterial : unhighlightedMaterial;
		}
	}

	public bool IsOccupied {
		get {
			return occupied;
		}

		private set {
			occupied = value;
		}
	}

	public bool IsEntryPoint {
		get {
			return entryPoint;
		}

		set {
			entryPoint = IsOccupied = value;
			renderer.material = unhighlightedMaterial = (entryPoint) ? entryPointMaterial : unhighlightedMaterial;
			RescaleHeight((entryPoint) ? roadHeight : defaultHeight);
		}
	}

	public bool IsExitPoint {
		get {
			return exitPoint;
		}

		set {
			exitPoint = IsOccupied = value;
			renderer.material = unhighlightedMaterial = (exitPoint) ? exitPointMaterial : unhighlightedMaterial;
			RescaleHeight((exitPoint) ? roadHeight : defaultHeight);
		}
	}

	public bool IsHighlighted {
		get {
			return highlighted;
		}

		set {
			highlighted = value;
			if (highlighted) {
				renderer.material = highlightedMaterial;
			} else {
				renderer.material = unhighlightedMaterial;
			}
		}
	}

	public bool IsGrass {
		get {
			return grass;
		}

		set {
			// Grass can only be on top of stack grids
			if (!IsTopOfStack) {
				return;
			}

			// Entry and exit points should not have grass on top
			if (IsEntryPoint || IsExitPoint) {
				return;
			}

			if (value) {
				// Override road grid
				RoadHorizontalDirection = 0;
				RoadVerticalDirection = 0;
			}

			grass = IsOccupied = value;

			UpdateGrassMaterial();
		}
	}

	public int RoadHorizontalDirection {
		get {
			return roadHorizontalDirection;
		}

		set {
			// Road can only be built on top of stack grids
			if (!IsTopOfStack) {
				return;
			}

			// Entry and exit points should not have roads built on top
			if (IsEntryPoint || IsExitPoint) {
				return;
			}

			if (value != 0) {
				// Override grass grid
				IsGrass = false;
			}

			roadHorizontalDirection = value;
			IsOccupied = (roadHorizontalDirection != 0 || roadVerticalDirection != 0);

			UpdateRoadMaterial();
		}
	}

	public int RoadVerticalDirection {
		get {
			return roadVerticalDirection;
		}

		set {
			// Road can only be built on top of stack grids
			if (!IsTopOfStack) {
				return;
			}

			// Entry and exit points should not have roads built on top
			if (IsEntryPoint || IsExitPoint) {
				return;
			}

			if (value != 0) {
				// Override grass grid
				IsGrass = false;
			}

			roadVerticalDirection = value;
			IsOccupied = (roadHorizontalDirection != 0 || roadVerticalDirection != 0);

			UpdateRoadMaterial();
		}
	}

	public bool IsHidden {
		get {
			return renderer.enabled;
		}

		set {
			renderer.enabled = !value;
		}
	}

	private void Awake() {
		renderer = GetComponentInChildren<Renderer>();

		unhighlightedMaterial = defaultMaterial;

		neighbours = new Dictionary<Vector3, Grid>();

		IsPersistent = false;
		IsTopOfStack = false;
		IsOccupied = false;
		IsEntryPoint = false;
		IsExitPoint = false;
		IsHighlighted = false;
		IsGrass = false;
		roadHorizontalDirection = 0;
		RoadVerticalDirection = 0;
	}

	private void UpdateRoadMaterial() {
		if (roadHorizontalDirection == -1 && roadVerticalDirection == -1) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = backwardAndLeftwardMaterial;
		} else if (roadHorizontalDirection == -1 && roadVerticalDirection == 0) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = leftwardMaterial;
		} else if (roadHorizontalDirection == -1 && roadVerticalDirection == 1) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = forwardAndLeftwardMaterial;
		} else if (roadHorizontalDirection == 0 && roadVerticalDirection == -1) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = backwardMaterial;
		} else if (roadHorizontalDirection == 0 && roadVerticalDirection == 1) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = forwardMaterial;
		} else if (roadHorizontalDirection == 1 && roadVerticalDirection == -1) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = backwardAndRightwardMaterial;
		} else if (roadHorizontalDirection == 1 && roadVerticalDirection == 0) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = rightwardMaterial;
		} else if (roadHorizontalDirection == 1 && roadVerticalDirection == 1) {
			RescaleHeight(roadHeight);
			renderer.material = unhighlightedMaterial = forwardAndRightwardMaterial;
		} else {
			RescaleHeight(defaultHeight);
			renderer.material = unhighlightedMaterial = topOfStackMaterial;
		}
	}

	private void UpdateGrassMaterial() {
		if (grass) {
			RescaleHeight(grassHeight);
			renderer.material = unhighlightedMaterial = grassMaterial;
		} else {
			RescaleHeight(defaultHeight);
			renderer.material = unhighlightedMaterial = topOfStackMaterial;
		}
	}

	private void RescaleHeight(float newHeight) {
		transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);
	}*/

}
