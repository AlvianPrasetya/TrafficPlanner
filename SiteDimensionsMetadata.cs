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

	public SiteDimensionsMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(COORDINATES_SEPARATOR);

		siteDimensions = new Vector3(
			int.Parse(metadataTokens[0]), 
			int.Parse(metadataTokens[1]), 
			int.Parse(metadataTokens[2]));
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
