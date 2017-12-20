public interface ISelectable {

	bool IsSelectable {
		get;
	}

	void Select();

	void Unselect();

}
