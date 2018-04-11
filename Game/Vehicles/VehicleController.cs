using UnityEngine;
using System.Collections.Generic;

public class VehicleController : MonoBehaviour, ICollectible, IObservable<VehicleController, float> {

	public float roadEndReachedThreshold;
	public float roadEndSlowDownThreshold;
	public float maxSpeed;
	public float acceleration;
	public float deceleration;
	public float queueingRadius;
	public float brakingDistance;

	private LinkedListNode<Road> currentRoadNode;

	private float currentSpeed;
	private float targetSpeed;

	private List<Observer<VehicleController, float>> speedObservers;

	public void Enlist() {
		SiteManager.Instance.simulationManager.OnVehicleSpawnedCallback();
		SiteManager.Instance.trafficManager.AddSimulatedVehicle(this);
	}

	public void Delist() {
		SiteManager.Instance.simulationManager.OnVehicleReachedExitCallback();
		SiteManager.Instance.trafficManager.RemoveSimulatedVehicle(this);
	}

	public void AddObserver(Observer<VehicleController, float> observer) {
		speedObservers.Add(observer);
	}

	public void RemoveObserver(Observer<VehicleController, float> observer) {
		speedObservers.Remove(observer);
	}

	public void ClearObservers() {
		speedObservers.Clear();
	}

	public void NotifyObservers(float speed) {
		foreach (Observer<VehicleController, float> speedObserver in speedObservers) {
			speedObserver.Notify(this, speed);
		}
	}

	public void Initialize(LinkedListNode<Road> firstRoadNode) {
		currentRoadNode = firstRoadNode;

		currentSpeed = 0.0f;
		targetSpeed = 0.0f;
	}
	
	public float SqrDistFromRoadEnd {
		get {
			Road currentRoad = currentRoadNode.Value;
			return Vector3.SqrMagnitude(currentRoad.EndGrid.transform.position - transform.position);
		}
	}

	private void Awake() {
		speedObservers = new List<Observer<VehicleController, float>>();

		Enlist();
	}

	private void Update() {
		if (currentRoadNode == null) {
			return;
		}

		if (SqrDistFromRoadEnd < roadEndReachedThreshold * roadEndReachedThreshold) {
			// Reached road end, enumerate to the next road in the path
			currentRoadNode = currentRoadNode.Next;
			if (currentRoadNode == null) {
				// If end of path is reached, despawn vehicle
				Destroy(gameObject);
				return;
			}
		}

		targetSpeed = EvaluateTargetSpeed();

		// Notify all observers of the current speed
		NotifyObservers(currentSpeed);
	}

	private void FixedUpdate() {
		Throttle();
		TraversePath();
	}

	private void OnDestroy() {
		Delist();
	}

	private float EvaluateTargetSpeed() {
		Collider[] vehiclesWithinRange = Physics.OverlapSphere(transform.position, queueingRadius, LayerUtils.Mask.VEHICLE);

		foreach (Collider vehicleWithinRange in vehiclesWithinRange) {
			VehicleController other = vehicleWithinRange.transform.GetComponent<VehicleController>();

			if (other == this) {
				continue;
			}

			float thisDistanceToIntersection, otherDistanceToIntersection;
			CalculateDistanceToIntersection(other, out thisDistanceToIntersection, out otherDistanceToIntersection);
			/*float thisDistanceToIntersection = GetDistanceToIntersection(other);
			float otherDistanceToIntersection = other.GetDistanceToIntersection(this);*/
			// Check whether this vehicle needs to give way to the other vehicle
			if (thisDistanceToIntersection < brakingDistance && otherDistanceToIntersection < brakingDistance) {
				if ((thisDistanceToIntersection == otherDistanceToIntersection && GetInstanceID() > other.GetInstanceID())
					|| thisDistanceToIntersection > otherDistanceToIntersection) {
					return 0.0f;
				}
			}
		}

		// Check if vehicle should slow down to make a turn
		if (SqrDistFromRoadEnd < roadEndSlowDownThreshold * roadEndSlowDownThreshold && currentRoadNode.Next != null) {
			Road currentRoad = currentRoadNode.Value;
			Road nextRoad = currentRoadNode.Next.Value;
			float turningAngle = Vector3.Angle(currentRoad.RoadDirection, nextRoad.RoadDirection);
			return maxSpeed * Mathf.Sqrt(1 - Mathf.Pow(turningAngle / 180.0f, 2));
		}

		return maxSpeed;
	}

	private void CalculateDistanceToIntersection(VehicleController other, 
		out float distanceToIntersection, out float otherDistanceToIntersection) {
		distanceToIntersection = 0.0f;
		otherDistanceToIntersection = 0.0f;
		for (LinkedListNode<Road> thisCurrent = currentRoadNode; thisCurrent != null; thisCurrent = thisCurrent.Next) {
			// Append current road to intersection distance
			if (thisCurrent.Value == currentRoadNode.Value) {
				distanceToIntersection += Mathf.Sqrt(SqrDistFromRoadEnd);
			} else {
				distanceToIntersection += thisCurrent.Value.RoadLength;
			}
			
			for (LinkedListNode<Road> otherCurrent = other.currentRoadNode; otherCurrent != null; otherCurrent = otherCurrent.Next) {
				// Append other's current road to other intersection distance
				if (otherCurrent.Value == other.currentRoadNode.Value) {
					otherDistanceToIntersection += Mathf.Sqrt(other.SqrDistFromRoadEnd);
				} else {
					otherDistanceToIntersection += otherCurrent.Value.RoadLength;
				}

				if (thisCurrent.Value.EndGrid == otherCurrent.Value.EndGrid) {
					// Intersection found
					if (thisCurrent.Value == otherCurrent.Value) {
						// Special case: Intersection at the same road
						if (currentRoadNode.Value == other.currentRoadNode.Value) {
							// Intersection at both vehicle's initial road
							if (SqrDistFromRoadEnd > other.SqrDistFromRoadEnd) {
								// Other vehicle is closer to the road end, intersection is at other vehicle's position
								distanceToIntersection -= Mathf.Sqrt(other.SqrDistFromRoadEnd);
								otherDistanceToIntersection -= Mathf.Sqrt(other.SqrDistFromRoadEnd);
							} else {
								// This vehicle is closer to the road end, intersection is at this vehicle's position
								distanceToIntersection -= Mathf.Sqrt(SqrDistFromRoadEnd);
								otherDistanceToIntersection -= Mathf.Sqrt(SqrDistFromRoadEnd);
							}
						} else if (otherCurrent.Value == other.currentRoadNode.Value) {
							// Intersection at the other vehicle's initial road
							distanceToIntersection -= Mathf.Sqrt(other.SqrDistFromRoadEnd);
							otherDistanceToIntersection -= Mathf.Sqrt(other.SqrDistFromRoadEnd);
						} else if (thisCurrent.Value == currentRoadNode.Value) {
							// Intersection at this vehicle's initial road
							distanceToIntersection -= Mathf.Sqrt(SqrDistFromRoadEnd);
							otherDistanceToIntersection -= Mathf.Sqrt(SqrDistFromRoadEnd);
						}
					}

					return;
				}
			}
		}

		distanceToIntersection = otherDistanceToIntersection = Mathf.Infinity;
		return;
	}

	private void Throttle() {
		if (currentSpeed < targetSpeed) {
			currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.fixedDeltaTime, 0.0f, targetSpeed);
		} else if (currentSpeed > targetSpeed) {
			currentSpeed = Mathf.Clamp(currentSpeed - deceleration * Time.fixedDeltaTime, 0.0f, targetSpeed);
		} else {
			// Target speed reached, do nothing
		}
	}

	private void TraversePath() {
		if (currentRoadNode == null) {
			return;
		}

		Road currentRoad = currentRoadNode.Value;
		
		Vector3 forward = (currentRoad.EndGrid.transform.position - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(forward, currentRoad.transform.up);
		transform.Translate(Vector3.forward * currentSpeed * Time.fixedDeltaTime);
	}

}
