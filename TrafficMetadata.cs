using UnityEngine;
using System.Collections.Generic;

public class TrafficMetadata {

	public static readonly string METADATA_QUALIFIER = "tr";
	public static readonly char METADATA_SEPARATOR = '_';
	private static readonly char EXIT_POINTS_SEPARATOR = '-';
	private static readonly char COORDINATES_SEPARATOR = ',';

	private Vector3 entryCoordinates;
	private List<Vector3> exitCoordinateList;
	private List<float> trafficDistributionList;
	private int trafficVolume;

	public Vector3 EntryCoordinates {
		get {
			return entryCoordinates;
		}
	}

	public IEnumerable<Vector3> ExitCoordinateList {
		get {
			return exitCoordinateList;
		}
	}

	public IEnumerable<float> TrafficVolumeList {
		get {
			return trafficDistributionList;
		}
	}

	public int TrafficVolume {
		get {
			return trafficVolume;
		}
	}

	public TrafficMetadata(string metadataString) {
		string unqualifiedMetadataString = metadataString.Replace(METADATA_QUALIFIER + METADATA_SEPARATOR, "");
		string[] metadataTokens = unqualifiedMetadataString.Split(METADATA_SEPARATOR);

		string[] entryCoordinateTokens = metadataTokens[0].Split(COORDINATES_SEPARATOR);
		entryCoordinates = new Vector3(
			int.Parse(entryCoordinateTokens[0]), 
			int.Parse(entryCoordinateTokens[1]), 
			int.Parse(entryCoordinateTokens[2]));

		exitCoordinateList = new List<Vector3>();
		trafficDistributionList = new List<float>();
		string[] exitPointTokens = metadataTokens[1].Split(EXIT_POINTS_SEPARATOR);
		foreach (string exitPointToken in exitPointTokens) {
			string[] exitCoordinateTokens = exitPointToken.Split(COORDINATES_SEPARATOR);
			exitCoordinateList.Add(new Vector3(
				int.Parse(exitCoordinateTokens[0]), 
				int.Parse(exitCoordinateTokens[1]), 
				int.Parse(exitCoordinateTokens[2])));

			trafficDistributionList.Add(float.Parse(exitCoordinateTokens[3]));
		}

		trafficVolume = int.Parse(metadataTokens[2]);
	}

	public static bool IsQualified(string metadataString) {
		return metadataString.StartsWith(METADATA_QUALIFIER);
	}

}
