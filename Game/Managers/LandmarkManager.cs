using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandmarkManager : MonoBehaviour {

	public LandmarkBuilder landmarkBuilder;

	private HashSet<Landmark> landmarks;

	public void AddLandmark(Landmark landmark) {
		landmarks.Add(landmark);
	}

	public void RemoveLandmark(Landmark landmark) {
		landmarks.Remove(landmark);
	}

	public IEnumerator GenerateLandmarks(List<LandmarkMetadata> landmarkMetadataList) {
		UIManager.Instance.Prompt("Generating landmarks...");

		foreach (LandmarkMetadata landmarkMetadata in landmarkMetadataList) {
			landmarkBuilder.BuildLandmark(
				landmarkMetadata.LandmarkAssetId,
				SiteManager.Instance.gridManager.GetGrid(landmarkMetadata.LandmarkStart), 
				SiteManager.Instance.gridManager.GetGrid(landmarkMetadata.LandmarkEnd));

			yield return null;
		}
	}

	public List<LandmarkMetadata> GenerateMetadata() {
		List<LandmarkMetadata> landmarksMetadata = new List<LandmarkMetadata>();
		foreach (Landmark landmark in landmarks) {
			landmarksMetadata.Add(new LandmarkMetadata(
				landmark.assetId, landmark.StartGrid.Coordinates, landmark.EndGrid.Coordinates));
		}

		return landmarksMetadata;
	}

	private void Awake() {
		landmarks = new HashSet<Landmark>();
	}

}
