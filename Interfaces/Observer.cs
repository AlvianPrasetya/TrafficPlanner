using UnityEngine;

public abstract class Observer<T, U> : MonoBehaviour {

	public abstract void Notify(T sender, U data);

}
