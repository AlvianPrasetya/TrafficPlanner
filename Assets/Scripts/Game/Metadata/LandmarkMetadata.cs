using UnityEngine;

public class LandmarkMetadata {

	public static readonly string METADATA_QUALIFIER = "lm";
	public static readonly char METADATA_SEPARATOR = '_';
	private static readonly char COORDINATES_SEPARATOR = ',';

	private int landmarkAssetId;
	private Vector3 landmarkStart;
	private Vector3 landmarkEnd;

	public int LandmarkAssetId {
		get {
			return landmarkAssetId;
		}
	}

	public Vector3 LandmarkStart {
		get {
			return landmarkStart;
		}
	}

	public Vector3 LandmarkEnd {
		get {
			return landmarkEnd;
		}
	}

	public override string ToString() {
		string metadata = METADATA_QUALIFIER;
		metadata += METADATA_SEPARATOR;

		metadata += landmarkAssetId;
		metadata += METADATA_SEPARATOR;
			
		metadata += (int) landmarkStart.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) landmarkStart.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) landmarkStart.z;
		metadata += METADATA_SEPARATOR;

		metadata += (int) landmarkEnd.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) landmarkEnd.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) landmarkEnd.z;

		return metadata;
	}

	public LandmarkMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(METADATA_SEPARATOR);

		landmarkAssetId = int.Parse(metadataTokens[0]);
		string[] landmarkStartTokens = metadataTokens[1].Split(COORDINATES_SEPARATOR);
		string[] landmarkEndTokens = metadataTokens[2].Split(COORDINATES_SEPARATOR);

		landmarkStart = new Vector3(
			int.Parse(landmarkStartTokens[0]),
			int.Parse(landmarkStartTokens[1]),
			int.Parse(landmarkStartTokens[2]));

		landmarkEnd = new Vector3(
			int.Parse(landmarkEndTokens[0]),
			int.Parse(landmarkEndTokens[1]),
			int.Parse(landmarkEndTokens[2]));
	}

	public LandmarkMetadata(int landmarkAssetId, Vector3 landmarkStart, Vector3 landmarkEnd) {
		this.landmarkAssetId = landmarkAssetId;
		this.landmarkStart = landmarkStart;
		this.landmarkEnd = landmarkEnd;
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
