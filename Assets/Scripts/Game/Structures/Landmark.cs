using System;
using System.Linq;
using UnityEngine;

public class Landmark : MonoBehaviour, IBigridTransform, ICollectible, IDemolishable {

	public int assetId;
	public Material selectedMaterial;
	public Material demolishMaterial;

	public bool selectable;
	public bool demolishable;

	protected Grid startGrid;
	protected Grid endGrid;

	private Renderer[] renderers;
	private Material[] rendererMaterials;

	public Grid StartGrid {
		get {
			return startGrid;
		}

		set {
			startGrid = value;
		}
	}

	public Grid EndGrid {
		get {
			return endGrid;
		}

		set {
			endGrid = value;
			
			if (startGrid != null && endGrid != null) {
				Vector3 diff = endGrid.Coordinates - startGrid.Coordinates;
				Vector3 diffSign = new Vector3(Mathf.Sign(diff.x), Mathf.Sign(diff.y), Mathf.Sign(diff.z));
				transform.localScale = diff + diffSign;
				transform.position = startGrid.transform.position
					+ Vector3.Scale(new Vector3(0.5f, 0.0f, 0.5f), -diffSign);

				// Set grids as occupied
				int xMin = (int) Mathf.Min(startGrid.Coordinates.x, endGrid.Coordinates.x);
				int xMax = (int) Mathf.Max(startGrid.Coordinates.x, endGrid.Coordinates.x);

				int yMin = (int) Mathf.Min(startGrid.Coordinates.y, endGrid.Coordinates.y);
				int yMax = (int) Mathf.Max(startGrid.Coordinates.y, endGrid.Coordinates.y);

				int zMin = (int) Mathf.Min(startGrid.Coordinates.z, endGrid.Coordinates.z);
				int zMax = (int) Mathf.Max(startGrid.Coordinates.z, endGrid.Coordinates.z);

				for (int x = xMin; x <= xMax; x++) {
					for (int y = yMin; y <= yMax; y++) {
						for (int z = zMin; z <= zMax; z++) {
							Grid currentGrid = SiteManager.Instance.gridManager.GetGrid(x, y, z);
							if (currentGrid.State == Grid.GridState.OCCUPIED) {
								// TODO: Handle exception here, a landmark grid is already occupied
							} else {
								currentGrid.State = Grid.GridState.OCCUPIED;
							}
						}
					}
				}
			}
		}
	}

	public void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail) {
		throw new NotImplementedException("Landmark BigridTransform could not be split");
	}

	public void Enlist() {
		SiteManager.Instance.landmarkManager.AddLandmark(this);
	}

	public void Delist() {
		SiteManager.Instance.landmarkManager.RemoveLandmark(this);
	}

	public bool IsSelectable {
		get {
			return selectable;
		}
	}

	public void Select() {
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].material = selectedMaterial;
		}
	}

	public void Unselect() {
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].material = rendererMaterials[i];
		}
	}

	public bool IsDemolishable {
		get {
			return demolishable;
		}
	}

	public void IndicateDemolish() {
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].material = demolishMaterial;
		}
	}

	public void UnindicateDemolish() {
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].material = rendererMaterials[i];
		}
	}

	public void Demolish() {
		Delist();

		// Set grids as empty
		int xMin = (int) Mathf.Min(startGrid.Coordinates.x, endGrid.Coordinates.x);
		int xMax = (int) Mathf.Max(startGrid.Coordinates.x, endGrid.Coordinates.x);

		int yMin = (int) Mathf.Min(startGrid.Coordinates.y, endGrid.Coordinates.y);
		int yMax = (int) Mathf.Max(startGrid.Coordinates.y, endGrid.Coordinates.y);

		int zMin = (int) Mathf.Min(startGrid.Coordinates.z, endGrid.Coordinates.z);
		int zMax = (int) Mathf.Max(startGrid.Coordinates.z, endGrid.Coordinates.z);

		for (int x = xMin; x <= xMax; x++) {
			for (int y = yMin; y <= yMax; y++) {
				for (int z = zMin; z <= zMax; z++) {
					Grid currentGrid = SiteManager.Instance.gridManager.GetGrid(x, y, z);
					if (currentGrid.State == Grid.GridState.EMPTY) {
						// TODO: Handle exception here, a landmark grid is already empty
					} else {
						currentGrid.State = Grid.GridState.EMPTY;
					}
				}
			}
		}

		Destroy(gameObject);
	}

	private void Awake() {
		Enlist();

		renderers = GetComponentsInChildren<Renderer>();
		rendererMaterials = renderers.Select(i => i.material).ToArray();
	}

}
