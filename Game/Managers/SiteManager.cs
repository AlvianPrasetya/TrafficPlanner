using UnityEngine;
using System.Collections;

public class SiteManager : MonoBehaviour {

	public GridManager gridManager;
	public LandmarkManager landmarkManager;
	public TrafficManager trafficManager;
	public RoadManager roadManager;
	public SimulationManager simulationManager;

	public Ground groundPrefab;

	private static SiteManager instance;

	public static SiteManager Instance {
		get {
			return instance;
		}
	}

	private void Awake() {
		instance = this;
	}

	public IEnumerator GenerateSite(LevelMetadata levelMetadata, LevelMetadata attemptMetadata) {
		UIManager.Instance.Prompt("Generating level...");

		// Generate level based on level metadata
		yield return GenerateSite(
			(int) levelMetadata.SiteDimensionsMetadata.SiteDimensions.x,
			(int) levelMetadata.SiteDimensionsMetadata.SiteDimensions.y,
			(int) levelMetadata.SiteDimensionsMetadata.SiteDimensions.z);
		yield return landmarkManager.GenerateLandmarks(levelMetadata.LandmarkMetadataList);
		yield return trafficManager.GenerateAccessPoints(levelMetadata.TrafficMetadataList);
		yield return roadManager.GenerateArterials(levelMetadata.RoadMetadataList);

		// Generate site based on attempt metadata
		yield return roadManager.GenerateRoads(attemptMetadata.RoadMetadataList);

		UIManager.Instance.Prompt("Level generated successfully");
	}

	public IEnumerator GenerateSite(int xDimension, int yDimension, int zDimension) {
		yield return GenerateGround(xDimension, yDimension, zDimension);
		yield return gridManager.GenerateGrids(xDimension, yDimension, zDimension);
	}

	public LevelMetadata GenerateAttemptMetadata() {
		return new LevelMetadata(
			null,
			null,
			null,
			roadManager.GenerateAttemptMetadata());
	}

	public LevelMetadata GenerateLevelMetadata() {
		return new LevelMetadata(
			gridManager.GenerateMetadata(),
			landmarkManager.GenerateMetadata(),
			trafficManager.GenerateMetadata(),
			roadManager.GenerateLevelMetadata());
	}

	private IEnumerator GenerateGround(int xDimension, int yDimension, int zDimension) {
		UIManager.Instance.Prompt("Generating ground...");

		Ground ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity);

		yield return CoroutineUtils.ScaleWithDuration(
			ground.transform,
			new Vector3(xDimension, 1.0f, zDimension),
			0.5f);
	}

}
