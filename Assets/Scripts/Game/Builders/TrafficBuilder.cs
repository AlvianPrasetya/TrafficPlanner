using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficBuilder : AccessGridSelector {

	public RoadBuilder roadBuilder;

	public TrafficPreview trafficPreviewPrefab;

	private enum State {
		IDLE,
		BUILDING
	}

	private State state;

	private TrafficPreview trafficPreview;

	private Dictionary<Grid, EntryRoad> entryRoadsAt;
	private Dictionary<Grid, ExitRoad> exitRoadsAt;

	private float trafficVolume;

	public float TrafficVolume {
		set {
			trafficVolume = value;
		}
	}

	public IEnumerable<EntryRoad> EntryRoads {
		get {
			return entryRoadsAt.Select(i => i.Value);
		}
	}

	public IEnumerable<ExitRoad> ExitRoads {
		get {
			return exitRoadsAt.Select(i => i.Value);
		}
	}

	public void AddEntryRoad(EntryRoad entryRoad) {
		entryRoadsAt[entryRoad.EndGrid] = entryRoad;
	}

	public void RemoveEntryRoad(EntryRoad entryRoad) {
		entryRoadsAt.Remove(entryRoad.EndGrid);
	}

	public void AddExitRoad(ExitRoad exitRoad) {
		exitRoadsAt[exitRoad.StartGrid] = exitRoad;
	}

	public void RemoveExitRoad(ExitRoad exitRoad) {
		exitRoadsAt.Remove(exitRoad.StartGrid);
	}

	protected override void Awake() {
		base.Awake();

		state = State.IDLE;

		entryRoadsAt = new Dictionary<Grid, EntryRoad>();
		exitRoadsAt = new Dictionary<Grid, ExitRoad>();
	}

	protected override void Update() {
		base.Update();

		if (!active) {
			return;
		}

		switch (state) {
			case State.IDLE:
				InputStartBuilding(selected);
				break;
			case State.BUILDING:
				UpdateAccessPointPreview(selected);
				InputEndBuilding();
				break;
			default:
				break;
		}
	}

	public void BuildTraffic(Grid entryGrid, Grid exitGrid, float trafficVolume) {
		EntryRoad entryRoad;
		if (!entryRoadsAt.TryGetValue(entryGrid, out entryRoad)) {
			entryRoad = roadBuilder.BuildEntryRoad(entryGrid.Coordinates);
			entryRoadsAt[entryGrid] = entryRoad;
		}

		ExitRoad exitRoad;
		if (!exitRoadsAt.TryGetValue(exitGrid, out exitRoad)) {
			exitRoad = roadBuilder.BuildExitRoad(exitGrid.Coordinates);
			exitRoadsAt[exitGrid] = exitRoad;
		}
		
		new Traffic(entryRoad, exitRoad, trafficVolume);
	}

	private void InputStartBuilding(Grid startGrid) {
		if (Input.GetMouseButtonDown(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (startGrid != null) {
				state = State.BUILDING;

				trafficPreview = Instantiate(trafficPreviewPrefab, 
					Vector3.zero, Quaternion.identity, 
					transform);
				trafficPreview.StartGrid = startGrid;

				trafficPreview.EmissionRate = 30;
				trafficPreview.ParticleSize = trafficVolume * 0.3f;
			} else {
				// Invalid start grid, cancel road building operation
				ResetState();
			}
		}
	}

	private void UpdateAccessPointPreview(Grid candidateEndGrid) {
		if (candidateEndGrid == trafficPreview.StartGrid) {
			return;
		}

		if (candidateEndGrid == trafficPreview.EndGrid) {
			return;
		}

		trafficPreview.EndGrid = candidateEndGrid;
		
		if (trafficPreview.IsValid != trafficPreview.IsActive) {
			trafficPreview.ToggleActive();
		}
	}

	private void InputEndBuilding() {
		if (Input.GetMouseButtonUp(InputUtils.MOUSE_BUTTON_LEFT)) {
			if (trafficPreview.IsValid) {
				BuildTraffic(trafficPreview.StartGrid, trafficPreview.EndGrid, trafficVolume);
			}

			// End access point building operation
			ResetState();
		}
	}

	private void ResetState() {
		state = State.IDLE;

		// Deactivate access point preview
		if (trafficPreview != null && trafficPreview.IsActive) {
			Destroy(trafficPreview.gameObject);
			trafficPreview = null;
		}
	}

}
