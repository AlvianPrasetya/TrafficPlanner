using UnityEngine;

public abstract class Structure : MonoBehaviour, IBigridTransform, ISelectable {

	public bool selectable;

	protected Grid startGrid;
	protected Grid endGrid;

	public abstract Grid StartGrid {
		get;
		set;
	}

	public abstract Grid EndGrid {
		get;
		set;
	}

	public abstract void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail);

	public bool IsSelectable {
		get {
			return selectable;
		}
	}

	public abstract void Select();

	public abstract void Unselect();

	public virtual void Demolish() {
		Destroy(gameObject);
	}

}
