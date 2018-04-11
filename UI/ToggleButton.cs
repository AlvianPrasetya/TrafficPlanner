using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour, IToggleable, IObservable<ToggleButton, bool> {

	public Image buttonBackground;
	public Color activeColor;
	public Color inactiveColor;

	public UnityEvent onActivated;
	public UnityEvent onDeactivated;

	private bool active;

	private List<Observer<ToggleButton, bool>> observers;

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
		buttonBackground.color = activeColor;
		NotifyObservers(true);

		onActivated.Invoke();
	}

	public virtual void OnDeactivated() {
		buttonBackground.color = inactiveColor;
		NotifyObservers(false);

		onDeactivated.Invoke();
	}

	public void AddObserver(Observer<ToggleButton, bool> observer) {
		observers.Add(observer);
	}

	public void RemoveObserver(Observer<ToggleButton, bool> observer) {
		observers.Remove(observer);
	}

	public void ClearObservers() {
		observers.Clear();
	}

	public void NotifyObservers(bool active) {
		foreach (Observer<ToggleButton, bool> observer in observers) {
			observer.Notify(this, active);
		}
	}

	private void Awake() {
		active = false;

		observers = new List<Observer<ToggleButton, bool>>();
	}

}
