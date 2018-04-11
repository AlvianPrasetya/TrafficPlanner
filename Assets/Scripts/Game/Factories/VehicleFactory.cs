using UnityEngine;

public class VehicleFactory : MonoBehaviour {

	public VehicleController[] vehicles;

	public VehicleController GetRandomVehicle() {
		if (vehicles.Length == 0) {
			return null;
		}

		return vehicles[Random.Range(0, vehicles.Length)];
	}

}
