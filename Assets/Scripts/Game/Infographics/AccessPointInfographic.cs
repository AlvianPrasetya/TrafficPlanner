using System.Collections.Generic;
using UnityEngine;

public class AccessPointInfographic : Infographic {

	public TrafficInfographic trafficInfographicPrefab;

	private List<TrafficInfographic> trafficInfographics;

	public override void OnActivated() {
		foreach (TrafficInfographic trafficInfographic in trafficInfographics) {
			if (!trafficInfographic.IsActive) {
				trafficInfographic.ToggleActive();
			}
		}
	}

	public override void OnDeactivated() {
		foreach (TrafficInfographic trafficInfographic in trafficInfographics) {
			if (trafficInfographic.IsActive) {
				trafficInfographic.ToggleActive();
			}
		}
	}

	public void AddTraffic(Traffic traffic) {
		TrafficInfographic trafficInfographic = Instantiate(trafficInfographicPrefab, 
			Vector3.zero, Quaternion.identity, 
			transform);
		trafficInfographic.StartGrid = traffic.EntryRoad.EndGrid;
		trafficInfographic.EndGrid = traffic.ExitRoad.StartGrid;

		trafficInfographic.EmissionRate = 30;
		trafficInfographic.ParticleSize = traffic.TrafficVolume * 0.3f;

		trafficInfographics.Add(trafficInfographic);
	}

	public void RemoveTraffic(Traffic traffic) {
		List<TrafficInfographic> trafficInfographicsCopy = new List<TrafficInfographic>(trafficInfographics);
		foreach (TrafficInfographic trafficInfographic in trafficInfographicsCopy) {
			if (trafficInfographic.StartGrid == traffic.EntryRoad.EndGrid
				&& trafficInfographic.EndGrid == traffic.ExitRoad.StartGrid) {
				Destroy(trafficInfographic.gameObject);

				trafficInfographics.Remove(trafficInfographic);
			}
		}
	}

	private void Awake() {
		trafficInfographics = new List<TrafficInfographic>();
	}

}
