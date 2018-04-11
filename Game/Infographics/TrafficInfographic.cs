using System;
using UnityEngine;

public class TrafficInfographic : Infographic, IBigridTransform {

	public ParticleSystem infographicParticleSystem;
	public float emissionAngle;

	private Grid startGrid;
	private Grid endGrid;
	
	private ParticleSystem.MainModule particleSystemMain;
	private ParticleSystem.EmissionModule particleSystemEmission;
	private ParticleSystemRenderer particleSystemRenderer;
	private float emissionAngleRad;

	public override void OnActivated() {
		infographicParticleSystem.Play();
	}

	public override void OnDeactivated() {
		infographicParticleSystem.Stop();
	}

	public virtual Grid StartGrid {
		get {
			return startGrid;
		}

		set {
			startGrid = value;

			transform.position = startGrid.transform.position;
		}
	}

	public virtual Grid EndGrid {
		get {
			return endGrid;
		}

		set {
			endGrid = value;

			if (startGrid != null && endGrid != null) {
				Vector3 planarDiffVector = new Vector3(endGrid.transform.position.x, 0.0f, endGrid.transform.position.z)
					- new Vector3(startGrid.transform.position.x, 0.0f, startGrid.transform.position.z);

				if (planarDiffVector != Vector3.zero) {
					transform.rotation = Quaternion.LookRotation(planarDiffVector.normalized, Vector3.up);
				} else {
					transform.rotation = Quaternion.identity;
				}

				float horizontalDistance = planarDiffVector.magnitude;
				float verticalDistance = endGrid.transform.position.y - startGrid.transform.position.y;
				float gravity = particleSystemMain.gravityModifier.constant * Physics.gravity.y;

				// Calculate initial speed of particle system to reach end grid
				float startSpeed = Mathf.Sqrt(0.5f * gravity * Mathf.Pow(horizontalDistance, 2)
					/ (verticalDistance * Mathf.Pow(Mathf.Cos(emissionAngleRad), 2)
					- horizontalDistance * Mathf.Sin(emissionAngleRad) * Mathf.Cos(emissionAngleRad)));

				float startLifetime = horizontalDistance / (startSpeed * Mathf.Cos(emissionAngleRad));

				particleSystemMain.startSpeed = startSpeed;
				particleSystemMain.startLifetime = startLifetime;
			}
		}
	}

	public float EmissionRate {
		set {
			particleSystemEmission.rateOverTime = value;
		}
	}

	public float ParticleSize {
		set {
			particleSystemMain.startSize = value;
		}
	}

	public Material ParticleMaterial {
		set {
			particleSystemRenderer.material = value;
		}
	}

	public void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail) {
		throw new NotImplementedException("AccessPointInfographic BigridTransform could not be split");
	}

	private void Awake() {
		particleSystemMain = infographicParticleSystem.main;
		particleSystemEmission = infographicParticleSystem.emission;
		particleSystemRenderer = infographicParticleSystem.GetComponent<ParticleSystemRenderer>();

		emissionAngleRad = emissionAngle * Mathf.PI / 180.0f;
	}

}
