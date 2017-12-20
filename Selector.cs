using UnityEngine;

public abstract class Selector : MonoBehaviour, IToggleable {

	public Camera referenceCamera;

	protected bool active;
	protected ISelectable selected;

	public void ToggleActive() {
		active = !active;

		if (active) {
			OnActivated();
		} else {
			OnDeactivated();
		}
	}

	public virtual void OnActivated() {

	}

	public virtual void OnDeactivated() {

	}

	protected virtual void Awake() {
		active = false;
	}

	protected virtual void Update() {
		if (!active) {
			return;
		}

		UpdateSelection();
	}

	protected abstract void UpdateSelection();

}
