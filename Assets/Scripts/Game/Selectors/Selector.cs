using UnityEngine;

public abstract class Selector<T> : MonoBehaviour, IToggleable {

	public Camera referenceCamera;
	
	protected bool active;
	protected T selected;

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
