using System;
using System.Linq;
using UnityEngine;

public class LandmarkPreview : MonoBehaviour, IBigridTransform, IValidatable {

	public int assetId;
	public Material invalidMaterial;

	private Grid startGrid;
	private Grid endGrid;

	private bool valid;

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
			}

			Validate();
		}
	}

	public void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail) {
		throw new NotImplementedException("Landmark BigridTransform could not be split");
	}

	public bool IsValid {
		get {
			return valid;
		}

		set {
			valid = value;

			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material = (valid) ? rendererMaterials[i] : invalidMaterial;
			}
		}
	}

	public void Validate() {
		if (startGrid == null || endGrid == null) {
			IsValid = false;
			return;
		}

		int xMin = (int) Mathf.Min(startGrid.Coordinates.x, endGrid.Coordinates.x);
		int xMax = (int) Mathf.Max(startGrid.Coordinates.x, endGrid.Coordinates.x);

		int yMin = (int) Mathf.Min(startGrid.Coordinates.y, endGrid.Coordinates.y);
		int yMax = (int) Mathf.Max(startGrid.Coordinates.y, endGrid.Coordinates.y);

		int zMin = (int) Mathf.Min(startGrid.Coordinates.z, endGrid.Coordinates.z);
		int zMax = (int) Mathf.Max(startGrid.Coordinates.z, endGrid.Coordinates.z);

		for (int x = xMin; x <= xMax; x++) {
			for (int y = yMin; y <= yMax; y++) {
				for (int z = zMin; z <= zMax; z++) {
					if (SiteManager.Instance.gridManager.GetGrid(x, y, z).State == Grid.GridState.OCCUPIED) {
						IsValid = false;
						return;
					}
				}
			}
		}

		IsValid = true;
		return;
	}

	private void Awake() {
		renderers = GetComponentsInChildren<Renderer>();
		rendererMaterials = renderers.Select(i => i.material).ToArray();
	}

}
