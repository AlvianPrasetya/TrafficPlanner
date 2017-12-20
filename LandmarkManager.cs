using UnityEngine;
using System.Collections.Generic;

public class LandmarkManager : MonoBehaviour {

	public LandmarkBuilder landmarkBuilder;

	private List<Landmark> landmarks;

	public void GenerateLandmarks(List<LandmarkMetadata> landmarkMetadataList) {
		Grid[,,] grids = SiteManager.Instance.gridManager.Grids;

		foreach (LandmarkMetadata landmarkMetadata in landmarkMetadataList) {
			int landmarkAssetId = landmarkMetadata.LandmarkAssetId;
			Vector3 landmarkPosition = landmarkMetadata.LandmarkPosition;
			Vector3 landmarkScale = landmarkMetadata.LandmarkScale;
			for (int x = (int) landmarkPosition.x; x < (int) (landmarkPosition.x + landmarkScale.x); x++) {
				for (int y = (int) landmarkPosition.y; y < (int) (landmarkPosition.y + landmarkScale.y); y++) {
					for (int z = (int) landmarkPosition.z; z < (int) (landmarkPosition.z + landmarkScale.z); z++) {
						if (grids[x, y, z].State == Grid.GridState.OCCUPIED) {
							// TODO: Handle exception here, a landmark grid is already occupied
						} else {
							grids[x, y, z].State = Grid.GridState.OCCUPIED;
						}
					}
				}
			}

			Landmark landmark = Instantiate(
				SiteManager.Instance.landmarkAssetManager.GetLandmarkAsset(landmarkAssetId),
				grids[(int) landmarkPosition.x, (int) landmarkPosition.y, (int) landmarkPosition.z].transform.position
				- new Vector3(0.5f, 0.0f, 0.5f),
				Quaternion.identity, 
				transform);
			landmark.transform.localScale = landmarkScale;

			landmarks.Add(landmark);
		}
	}

	private void Awake() {
		landmarks = new List<Landmark>();
	}

}
