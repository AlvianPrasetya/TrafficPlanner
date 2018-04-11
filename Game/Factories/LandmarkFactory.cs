using UnityEngine;

public class LandmarkFactory : MonoBehaviour {

	public LandmarkPreview[] landmarkPreviewPrefabs;
	public Landmark[] landmarkPrefabs;

	public LandmarkPreview GetLandmarkPreviewPrefab(int assetId) {
		foreach (LandmarkPreview landmarkPreviewPrefab in landmarkPreviewPrefabs) {
			if (landmarkPreviewPrefab.assetId == assetId) {
				return landmarkPreviewPrefab;
			}
		}

		return null;
	}

	public Landmark GetLandmarkPrefab(int assetId) {
		foreach (Landmark landmarkPrefab in landmarkPrefabs) {
			if (landmarkPrefab.assetId == assetId) {
				return landmarkPrefab;
			}
		}

		return null;
	}

}
