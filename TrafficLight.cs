using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrafficLight : MonoBehaviour {

	public enum TrafficPhase {
		FROM_NORTH = 0,
		FROM_EAST = 1,
		FROM_SOUTH = 2,
		FROM_WEST = 3,
		GRACE = 4, 
		IDLE = 5
	}

	public MeshRenderer northSignalMesh;
	public MeshRenderer eastSignalMesh;
	public MeshRenderer southSignalMesh;
	public MeshRenderer westSignalMesh;

	public Material redSignalMaterial;
	public Material yellowSignalMaterial;
	public Material greenSignalMaterial;

	public float checkInterval;
	public float northTrafficPeriod;
	public float eastTrafficPeriod;
	public float westTrafficPeriod;
	public float southTrafficPeriod;
	public float gracePeriod;

	private HashSet<VehicleController> trafficFromNorth;
	private HashSet<VehicleController> trafficFromEast;
	private HashSet<VehicleController> trafficFromSouth;
	private HashSet<VehicleController> trafficFromWest;

	public TrafficPhase currentPhase;

	private void Awake() {
		trafficFromNorth = new HashSet<VehicleController>();
		trafficFromEast = new HashSet<VehicleController>();
		trafficFromSouth = new HashSet<VehicleController>();
		trafficFromWest = new HashSet<VehicleController>();
	}

	private void Start() {
		StartCoroutine(PhaseChangeCoroutine());
	}

	private IEnumerator PhaseChangeCoroutine() {
		TrafficPhase lastTrafficPhase = TrafficPhase.IDLE;
		currentPhase = TrafficPhase.IDLE;
		UpdateSignals(lastTrafficPhase, currentPhase, currentPhase);

		while (true) {
			yield return new WaitForSecondsRealtime(checkInterval);

			TrafficPhase nextPhaseCandidate = EvaluateNextPhase(lastTrafficPhase);
			if (nextPhaseCandidate == currentPhase) {
				continue;
			}

			// Grace period before changing phase
			currentPhase = TrafficPhase.GRACE;
			UpdateSignals(lastTrafficPhase, currentPhase, nextPhaseCandidate);
			yield return new WaitForSecondsRealtime(gracePeriod);

			currentPhase = nextPhaseCandidate;
			UpdateSignals(lastTrafficPhase, currentPhase, nextPhaseCandidate);

			float waitPeriod = 0.0f;
			switch (currentPhase) {
				case TrafficPhase.FROM_NORTH:
					foreach (VehicleController vehicle in trafficFromNorth) {
						vehicle.IsHalted = false;
					}
					trafficFromNorth.Clear();
					waitPeriod = northTrafficPeriod;

					break;
				case TrafficPhase.FROM_EAST:
					foreach (VehicleController vehicle in trafficFromEast) {
						vehicle.IsHalted = false;
					}
					trafficFromEast.Clear();
					waitPeriod = eastTrafficPeriod;

					break;
				case TrafficPhase.FROM_SOUTH:
					foreach (VehicleController vehicle in trafficFromSouth) {
						vehicle.IsHalted = false;
					}
					trafficFromSouth.Clear();
					waitPeriod = southTrafficPeriod;

					break;
				case TrafficPhase.FROM_WEST:
					foreach (VehicleController vehicle in trafficFromWest) {
						vehicle.IsHalted = false;
					}
					trafficFromWest.Clear();
					waitPeriod = westTrafficPeriod;

					break;
				case TrafficPhase.IDLE:
					waitPeriod = 0.0f;

					break;
				default:
					break;
			}

			yield return new WaitForSecondsRealtime(waitPeriod);
			lastTrafficPhase = nextPhaseCandidate;
		}
	}

	private TrafficPhase EvaluateNextPhase(TrafficPhase currentPhase) {
		TrafficPhase phaseToEvaluate = currentPhase;
		HashSet<TrafficPhase> evaluated = new HashSet<TrafficPhase>();

		while (!evaluated.Contains(phaseToEvaluate = GetNextTrafficPhase(phaseToEvaluate))) {
			evaluated.Add(phaseToEvaluate);
			switch (phaseToEvaluate) {
				case TrafficPhase.FROM_NORTH:
					if (trafficFromNorth.Count != 0) {
						return TrafficPhase.FROM_NORTH;
					}

					break;
				case TrafficPhase.FROM_EAST:
					if (trafficFromEast.Count != 0) {
						return TrafficPhase.FROM_EAST;
					}

					break;
				case TrafficPhase.FROM_SOUTH:
					if (trafficFromSouth.Count != 0) {
						return TrafficPhase.FROM_SOUTH;
					}

					break;
				case TrafficPhase.FROM_WEST:
					if (trafficFromWest.Count != 0) {
						return TrafficPhase.FROM_WEST;
					}

					break;
				case TrafficPhase.GRACE:
					break;
				case TrafficPhase.IDLE:
					break;
				default:
					break;
			}
		}
		
		return TrafficPhase.IDLE;
	}

	private void OnTriggerEnter(Collider collider) {
		GameObject enteringGameObject = collider.gameObject;
		VehicleController enteringVehicle = enteringGameObject.GetComponent<VehicleController>();
		Vector3 enteringDirection = enteringVehicle.NextGrid.Coordinates - enteringVehicle.PrevGrid.Coordinates;

		if (enteringDirection == Vector3.back && currentPhase != TrafficPhase.FROM_NORTH) {
			trafficFromNorth.Add(enteringVehicle);
			enteringVehicle.IsHalted = true;
		} else if (enteringDirection == Vector3.left && currentPhase != TrafficPhase.FROM_EAST) {
			trafficFromEast.Add(enteringVehicle);
			enteringVehicle.IsHalted = true;
		} else if (enteringDirection == Vector3.forward && currentPhase != TrafficPhase.FROM_SOUTH) {
			trafficFromSouth.Add(enteringVehicle);
			enteringVehicle.IsHalted = true;
		} else if (enteringDirection == Vector3.right && currentPhase != TrafficPhase.FROM_WEST) {
			trafficFromWest.Add(enteringVehicle);
			enteringVehicle.IsHalted = true;
		}
	}

	private TrafficPhase GetNextTrafficPhase(TrafficPhase currentPhase) {
		return (TrafficPhase) (((int) currentPhase + 1) % (Enum.GetNames(typeof(TrafficPhase)).Length));
	}

	private void UpdateSignals(TrafficPhase previousPhase, TrafficPhase currentPhase, TrafficPhase nextPhase) {
		switch (previousPhase) {
			case TrafficPhase.FROM_NORTH:
				northSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : redSignalMaterial;
				break;
			case TrafficPhase.FROM_EAST:
				eastSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : redSignalMaterial;
				break;
			case TrafficPhase.FROM_SOUTH:
				southSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : redSignalMaterial;
				break;
			case TrafficPhase.FROM_WEST:
				westSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : redSignalMaterial;
				break;
			case TrafficPhase.IDLE:
				break;
			default:
				break;
		}

		switch (nextPhase) {
			case TrafficPhase.FROM_NORTH:
				northSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : greenSignalMaterial;
				break;
			case TrafficPhase.FROM_EAST:
				eastSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : greenSignalMaterial;
				break;
			case TrafficPhase.FROM_SOUTH:
				southSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : greenSignalMaterial;
				break;
			case TrafficPhase.FROM_WEST:
				westSignalMesh.material = (currentPhase == TrafficPhase.GRACE) ? yellowSignalMaterial : greenSignalMaterial;
				break;
			case TrafficPhase.IDLE:
				break;
			default:
				break;
		}
	}

}
