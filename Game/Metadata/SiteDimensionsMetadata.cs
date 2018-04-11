using UnityEngine;

public class SiteDimensionsMetadata {

	public static readonly string METADATA_QUALIFIER = "sd";
	public static readonly char METADATA_SEPARATOR = '_';
	private static readonly char COORDINATES_SEPARATOR = ',';

	private Vector3 siteDimensions;

	public Vector3 SiteDimensions {
		get {
			return siteDimensions;
		}
	}

	public override string ToString() {
		string metadata = METADATA_QUALIFIER;
		metadata += METADATA_SEPARATOR;

		metadata += (int) siteDimensions.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) siteDimensions.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) siteDimensions.z;

		return metadata;
	}

	public SiteDimensionsMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(COORDINATES_SEPARATOR);

		siteDimensions = new Vector3(
			int.Parse(metadataTokens[0]), 
			int.Parse(metadataTokens[1]), 
			int.Parse(metadataTokens[2]));
	}

	public SiteDimensionsMetadata(Vector3 siteDimensions) {
		this.siteDimensions = siteDimensions;
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
