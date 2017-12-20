using UnityEngine;

public class LandmarkAssetManager : MonoBehaviour {

	[System.Serializable]
	public class LandmarkAsset {

		public int landmarkAssetId;
		public Landmark landmarkPrefab;

	}

	public LandmarkAsset[] landmarkAssets;

	public Landmark GetLandmarkAsset(int landmarkAssetId) {
		foreach (LandmarkAsset landmarkAsset in landmarkAssets) {
			if (landmarkAsset.landmarkAssetId == landmarkAssetId) {
				return landmarkAsset.landmarkPrefab;
			}
		}

		return null;
	}

}
