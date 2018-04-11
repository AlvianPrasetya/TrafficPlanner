using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour {

	public RoadBuilder roadBuilder;

	private HashSet<ArterialRoad> arterials;
	private HashSet<Road> roads;

	public IEnumerable<ArterialRoad> Arterials {
		get {
			return arterials;
		}
	}

	public IEnumerable<Road> Roads {
		get {
			return roads;
		}
	}

	public void AddArterial(ArterialRoad arterial) {
		arterials.Add(arterial);
	}

	public void RemoveArterial(ArterialRoad arterial) {
		arterials.Remove(arterial);
	}

	public void AddRoad(Road road) {
		roads.Add(road);
	}

	public void RemoveRoad(Road road) {
		roads.Remove(road);
	}

	public IEnumerator GenerateArterials(List<RoadMetadata> roadMetadataList) {
		UIManager.Instance.Prompt("Generating arterials...");

		foreach (RoadMetadata roadMetadata in roadMetadataList) {
			roadBuilder.BuildArterial(
				SiteManager.Instance.gridManager.GetGrid(roadMetadata.RoadStart),
				SiteManager.Instance.gridManager.GetGrid(roadMetadata.RoadEnd));
			yield return null;
		}
	}

	public IEnumerator GenerateRoads(List<RoadMetadata> roadMetadataList) {
		UIManager.Instance.Prompt("Generating roads...");

		foreach (RoadMetadata roadMetadata in roadMetadataList) {
			roadBuilder.BuildRoad(
				SiteManager.Instance.gridManager.GetGrid(roadMetadata.RoadStart),
				SiteManager.Instance.gridManager.GetGrid(roadMetadata.RoadEnd));
			yield return null;
		}
	}

	public List<RoadMetadata> GenerateAttemptMetadata() {
		List<RoadMetadata> metadataList = new List<RoadMetadata>();
		foreach (Road road in roads) {
			metadataList.Add(new RoadMetadata(road.StartGrid.Coordinates, road.EndGrid.Coordinates));
		}

		return metadataList;
	}

	public List<RoadMetadata> GenerateLevelMetadata() {
		List<RoadMetadata> metadataList = new List<RoadMetadata>();
		foreach (ArterialRoad arterial in arterials) {
			metadataList.Add(new RoadMetadata(arterial.StartGrid.Coordinates, arterial.EndGrid.Coordinates));
		}

		return metadataList;
	}

	private void Awake() {
		arterials = new HashSet<ArterialRoad>();
		roads = new HashSet<Road>();
	}

}
