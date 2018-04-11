using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelMetadata {

	private static readonly char METADATA_SEPARATOR = '|';

	private SiteDimensionsMetadata siteDimensionsMetadata;
	private List<LandmarkMetadata> landmarkMetadataList;
	private List<TrafficMetadata> trafficMetadataList;
	private List<RoadMetadata> roadMetadataList;

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

	public List<TrafficMetadata> TrafficMetadataList {
		get {
			return trafficMetadataList;
		}
	}

	public List<RoadMetadata> RoadMetadataList {
		get {
			return roadMetadataList;
		}
	}

	public override string ToString() {
		IEnumerable<string> metadataStrings = new List<string>();

		if (siteDimensionsMetadata != null) {
			metadataStrings = metadataStrings.Concat(new string[] { siteDimensionsMetadata.ToString() });
		}

		if (landmarkMetadataList != null) {
			metadataStrings = metadataStrings.Concat(landmarkMetadataList.Select(i => i.ToString()));
		}

		if (trafficMetadataList != null) {
			metadataStrings = metadataStrings.Concat(trafficMetadataList.Select(i => i.ToString()));
		}

		if (roadMetadataList != null) {
			metadataStrings = metadataStrings.Concat(roadMetadataList.Select(i => i.ToString()));
		}

		return string.Join(METADATA_SEPARATOR.ToString(), metadataStrings.ToArray());
	}
	
	public LevelMetadata(string metadataString) {
		landmarkMetadataList = new List<LandmarkMetadata>();
		trafficMetadataList = new List<TrafficMetadata>();
		roadMetadataList = new List<RoadMetadata>();

		if (metadataString != null) {
			ParseMetadata(metadataString);
		}
	}

	public LevelMetadata(SiteDimensionsMetadata siteDimensionsMetadata, List<LandmarkMetadata> landmarkMetadataList,
		List<TrafficMetadata> trafficMetadataList, List<RoadMetadata> roadMetadataList) {
		this.siteDimensionsMetadata = siteDimensionsMetadata;
		this.landmarkMetadataList = landmarkMetadataList;
		this.trafficMetadataList = trafficMetadataList;
		this.roadMetadataList = roadMetadataList;
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
			} else if (RoadMetadata.IsQualified(metadataToken)) {
				roadMetadataList.Add(new RoadMetadata(metadataToken));
			} else {
				// Unknown metadata type
				Debug.Log("Error: unknown metadata type");
			}
		}
	}

}
