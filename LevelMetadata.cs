using UnityEngine;
using System.Collections.Generic;

public class LevelMetadata {

	private static readonly char METADATA_SEPARATOR = '|';

	private SiteDimensionsMetadata siteDimensionsMetadata;
	private List<LandmarkMetadata> landmarkMetadataList;
	private List<TrafficMetadata> trafficMetadataList;

	public SiteDimensionsMetadata SiteDimensionsMetadata {
		get {
			return siteDimensionsMetadata;
		}
	}

	public List<LandmarkMetadata> LandmarkMetadataList {
		get {
			return landmarkMetadataList;
		}
	}

	public List<TrafficMetadata> AccessPointMetadataList {
		get {
			return trafficMetadataList;
		}
	}
	
	public LevelMetadata(string metadataString) {
		landmarkMetadataList = new List<LandmarkMetadata>();
		trafficMetadataList = new List<TrafficMetadata>();

		ParseMetadata(metadataString);
	}

	private void ParseMetadata(string metadataString) {
		string[] metadataTokens = metadataString.Split(METADATA_SEPARATOR);

		foreach (string metadataToken in metadataTokens) {
			if (SiteDimensionsMetadata.IsQualified(metadataToken)) {
				siteDimensionsMetadata = new SiteDimensionsMetadata(metadataToken);
			} else if (LandmarkMetadata.IsQualified(metadataToken)) {
				landmarkMetadataList.Add(new LandmarkMetadata(metadataToken));
			} else if (TrafficMetadata.IsQualified(metadataToken)) {
				trafficMetadataList.Add(new TrafficMetadata(metadataToken));
			} else {
				// Unknown metadata type
				Debug.Log("Error: unknown metadata type");
			}
		}
	}

}
