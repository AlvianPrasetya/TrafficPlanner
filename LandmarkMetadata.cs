using UnityEngine;

public class LandmarkMetadata {

	public static readonly string METADATA_QUALIFIER = "lm";
	public static readonly char METADATA_SEPARATOR = '_';
	private static readonly char COORDINATES_SEPARATOR = ',';

	private int landmarkAssetId;
	private Vector3 landmarkPosition;
	private Vector3 landmarkScale;

	public int LandmarkAssetId {
		get {
			return landmarkAssetId;
		}
	}

	public Vector3 LandmarkPosition {
		get {
			return landmarkPosition;
		}
	}

	public Vector3 LandmarkScale {
		get {
			return landmarkScale;
		}
	}

	public LandmarkMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(METADATA_SEPARATOR);

		landmarkAssetId = int.Parse(metadataTokens[0]);
		string[] landmarkPositionTokens = metadataTokens[1].Split(COORDINATES_SEPARATOR);
		string[] landmarkScaleTokens = metadataTokens[2].Split(COORDINATES_SEPARATOR);

		landmarkPosition = new Vector3(
			int.Parse(landmarkPositionTokens[0]),
			int.Parse(landmarkPositionTokens[1]),
			int.Parse(landmarkPositionTokens[2]));

		landmarkScale = new Vector3(
			int.Parse(landmarkScaleTokens[0]),
			int.Parse(landmarkScaleTokens[1]),
			int.Parse(landmarkScaleTokens[2]));
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
