using UnityEngine;

public class RoadMetadata {

	public static readonly string METADATA_QUALIFIER = "rd";
	public static readonly char METADATA_SEPARATOR = '_';
	private static readonly char COORDINATES_SEPARATOR = ',';

	private Vector3 roadStart;
	private Vector3 roadEnd;

	public Vector3 RoadStart {
		get {
			return roadStart;
		}
	}

	public Vector3 RoadEnd {
		get {
			return roadEnd;
		}
	}

	public override string ToString() {
		string metadata = METADATA_QUALIFIER;
		metadata += METADATA_SEPARATOR;

		metadata += (int) roadStart.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) roadStart.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) roadStart.z;
		metadata += METADATA_SEPARATOR;

		metadata += (int) roadEnd.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) roadEnd.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) roadEnd.z;

		return metadata;
	}

	public RoadMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(METADATA_SEPARATOR);

		string[] roadStartTokens = metadataTokens[0].Split(COORDINATES_SEPARATOR);
		string[] roadEndTokens = metadataTokens[1].Split(COORDINATES_SEPARATOR);

		roadStart = new Vector3(
			int.Parse(roadStartTokens[0]),
			int.Parse(roadStartTokens[1]),
			int.Parse(roadStartTokens[2]));
		
		roadEnd = new Vector3(
			int.Parse(roadEndTokens[0]),
			int.Parse(roadEndTokens[1]),
			int.Parse(roadEndTokens[2]));
	}

	public RoadMetadata(Vector3 roadStart, Vector3 roadEnd) {
		this.roadStart = roadStart;
		this.roadEnd = roadEnd;
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
