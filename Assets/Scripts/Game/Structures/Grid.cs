using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour, ICollectible {

	public enum GridState {
		EMPTY, 
		BASE, 
		OCCUPIED, 
		SELECTED
	}

	public Renderer gridRenderer;

	public Material emptyMaterial;
	public Material selectableMaterial;
	public Material occupiedMaterial;
	public Material selectedMaterial;

	private GridState state;
	private Vector3 coordinates;
	private Dictionary<Vector3, Grid> neighbours;
	private GridState preSelectedState;

	public void Select() {
		preSelectedState = state;
		State = GridState.SELECTED;
	}

	public void Unselect() {
		if (state == GridState.SELECTED) {
			State = preSelectedState;
		}
	}

	public void Enlist() {
		SiteManager.Instance.gridManager.SetGrid(coordinates, this);
	}

	public void Delist() {
		SiteManager.Instance.gridManager.SetGrid(coordinates, null);
	}

	public GridState State {
		get {
			return state;
		}

		set {
			state = value;
			switch (state) {
				case GridState.EMPTY:
					gridRenderer.material = emptyMaterial;
					gameObject.layer = LayerUtils.Id.EMPTY_GRID;
					break;
				case GridState.BASE:
					gridRenderer.material = selectableMaterial;
					gameObject.layer = LayerUtils.Id.BASE_GRID;
					break;
				case GridState.OCCUPIED:
					gridRenderer.material = occupiedMaterial;
					gameObject.layer = LayerUtils.Id.OCCUPIED_GRID;
					break;
				case GridState.SELECTED:
					gridRenderer.material = selectedMaterial;
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
			gridRenderer.enabled = !value;
		}
	}

	private void Awake() {
		neighbours = new Dictionary<Vector3, Grid>();
	}

	private void OnDestroy() {
		Delist();
	}

}
