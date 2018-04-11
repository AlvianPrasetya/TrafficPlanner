using UnityEngine;

public abstract class Infographic : MonoBehaviour, IToggleable {

	protected bool active;

	public bool IsActive {
		get {
			return active;
		}
	}

	public void ToggleActive() {
		active = !active;

		if (active) {
			OnActivated();
		} else {
			OnDeactivated();
		}
	}

	public abstract void OnActivated();

	public abstract void OnDeactivated();

}
