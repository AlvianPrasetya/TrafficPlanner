public interface IToggleable {

	bool IsActive {
		get;
	}

	void ToggleActive();

	void OnActivated();

	void OnDeactivated();

}
