using System;
using UnityEngine;
using UnityEngine.Events;

public class Tab : MonoBehaviour, IToggleable {

	public UnityEvent onActivated;
	public UnityEvent onDeactivated;

	private bool active;

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
		onActivated.Invoke();
	}

	public virtual void OnDeactivated() {
		onDeactivated.Invoke();
	}

	private void Awake() {
		active = false;
	}

}
