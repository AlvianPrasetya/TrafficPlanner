using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour {

	public float tabButtonActiveY;
	public float tabButtonInactiveY;

	public Button[] tabButtons;
	public Tab[] tabs;

	public int activeTabId;

	public void SelectTab(int tabId) {
		if (tabId == activeTabId) {
			return; // Active tab is selected, do nothing
		}

		// Deactivate currently active tab
		if (tabButtons.Length > activeTabId && tabButtons[activeTabId] != null) {
			RectTransform rectTransform = tabButtons[activeTabId].GetComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(
				rectTransform.localPosition.x, tabButtonInactiveY, rectTransform.localPosition.z);
		}

		tabs[activeTabId].ToggleActive();
		tabs[activeTabId].gameObject.SetActive(false);

		// Activate new tab
		if (tabButtons.Length > tabId && tabButtons[tabId] != null) {
			RectTransform rectTransform = tabButtons[tabId].GetComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(
				rectTransform.localPosition.x, tabButtonActiveY, rectTransform.localPosition.z);
		}

		tabs[tabId].gameObject.SetActive(true);
		tabs[activeTabId].ToggleActive();

		// Change active tab
		activeTabId = tabId;
	}

	private void Awake() {
		for (int i = 0; i < tabButtons.Length; i++) {
			int tabId = i;
			tabButtons[i].onClick.AddListener(() => SelectTab(tabId));
		}
	}

}
