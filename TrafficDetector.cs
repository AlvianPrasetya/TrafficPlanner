using UnityEngine;
using System.Collections.Generic;

public class TrafficDetector : MonoBehaviour {

	private new Collider collider;

	private HashSet<VehicleController> detectedVehicles;

	public HashSet<VehicleController> DetectedVehicles {
		get {
			return detectedVehicles;
		}
	}

	private void Awake() {
		collider = GetComponent<Collider>();

		detectedVehicles = new HashSet<VehicleController>();
	}

	private void OnCollisionEnter(Collision collision) {
		GameObject enteringGameObject = collision.gameObject;
		VehicleController enteringVehicle = enteringGameObject.GetComponentInParent<VehicleController>();
		
		if (enteringVehicle != null) {
			detectedVehicles.Add(enteringVehicle);
		}
	}

	private void OnCollisionExit(Collision collision) {
		GameObject exitingGameObject = collision.gameObject;
		VehicleController exitingVehicle = exitingGameObject.GetComponentInParent<VehicleController>();

		if (exitingVehicle != null) {
			detectedVehicles.Remove(exitingVehicle);
		}
	}

}
