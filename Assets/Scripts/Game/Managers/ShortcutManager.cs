using UnityEngine;
using UnityEngine.UI;

public class ShortcutManager : MonoBehaviour {

	public Button[] buttons;
	public Button[] altButtons;
	public string[] shortcuts;

	private KeyCode[] keycodes;

	void Awake() {
		keycodes = new KeyCode[shortcuts.Length];

		for (int i = 0; i < shortcuts.Length; i++) {
			keycodes[i] = (KeyCode) System.Enum.Parse(typeof(KeyCode), shortcuts[i]);
		}
	}

	void Update() {
		for (int i = 0; i < keycodes.Length; i++) {
			if (Input.GetKeyDown(keycodes[i])) {
				buttons[i].onClick.Invoke();

				if (altButtons[i] != null && altButtons[i].IsActive()) {
					Button tmp = buttons[i];
					buttons[i] = altButtons[i];
					altButtons[i] = tmp;
				}
			}
		}
	}

}
