using UnityEngine;

public class TrafficMetadata {

	public static readonly string METADATA_QUALIFIER = "tr";
	public static readonly char METADATA_SEPARATOR = '_';
	private static readonly char COORDINATES_SEPARATOR = ',';

	private Vector3 entryCoordinates;
	private Vector3 exitCoordinates;
	private float trafficVolume;

	public Vector3 EntryCoordinates {
		get {
			return entryCoordinates;
		}
	}

	public Vector3 ExitCoordinates {
		get {
			return exitCoordinates;
		}
	}

	public float TrafficVolume {
		get {
			return trafficVolume;
		}
	}

	public override string ToString() {
		string metadata = METADATA_QUALIFIER;
		metadata += METADATA_SEPARATOR;

		metadata += (int) entryCoordinates.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) entryCoordinates.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) entryCoordinates.z;
		metadata += METADATA_SEPARATOR;
			
		metadata += (int) exitCoordinates.x;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) exitCoordinates.y;
		metadata += COORDINATES_SEPARATOR;
		metadata += (int) exitCoordinates.z;
		metadata += METADATA_SEPARATOR;

		metadata += trafficVolume;

		return metadata;
	}

	public TrafficMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(METADATA_SEPARATOR);

		string[] entryCoordinateTokens = metadataTokens[0].Split(COORDINATES_SEPARATOR);
		entryCoordinates = new Vector3(
			int.Parse(entryCoordinateTokens[0]), 
			int.Parse(entryCoordinateTokens[1]), 
			int.Parse(entryCoordinateTokens[2]));

		string[] exitCoordinateTokens = metadataTokens[1].Split(COORDINATES_SEPARATOR);
		exitCoordinates = new Vector3(
			int.Parse(exitCoordinateTokens[0]),
			int.Parse(exitCoordinateTokens[1]),
			int.Parse(exitCoordinateTokens[2]));

		trafficVolume = float.Parse(metadataTokens[2]);
	}

	public TrafficMetadata(Vector3 entryCoordinates, Vector3 exitCoordinates, float trafficVolume) {
		this.entryCoordinates = entryCoordinates;
		this.exitCoordinates = exitCoordinates;
		this.trafficVolume = trafficVolume;
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
