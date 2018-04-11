public class ToggleButtonGroup : Observer<ToggleButton, bool> {

	public ToggleButton[] toggleButtons;

	public override void Notify(ToggleButton toggledButton, bool active) {
		if (!active) {
			// Deactivation of toggle button, do nothing
			return;
		}

		// Activation of a toggle button, deactivate other toggle buttons
		foreach (ToggleButton toggleButton in toggleButtons) {
			if (toggleButton != toggledButton && toggleButton.IsActive) {
				toggleButton.ToggleActive();
			}
		}
	}

	// This function deactivates all toggle buttons whenever it is called
	public void Reset() {
		// Deactivate all toggle buttons
		foreach (ToggleButton toggleButton in toggleButtons) {
			if (toggleButton.IsActive) {
				toggleButton.ToggleActive();
			}
		}
	}

	private void Start() {
		foreach (ToggleButton toggleButton in toggleButtons) {
			toggleButton.AddObserver(this);
		}
	}

}
