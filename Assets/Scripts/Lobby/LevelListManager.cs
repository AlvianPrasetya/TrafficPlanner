using UnityEngine;
using System.Collections.Generic;

public class LevelListManager : MonoBehaviour {
	
	public LevelEntry levelEntryPrefab;
	public AttemptEntry attemptEntryPrefab;

	public RectTransform rootTransform;
	public Vector2 offset;
	public float verticalSpacing;

	public GameObject overlay;
	public RectTransform overlayRootTransform;
	public Vector2 overlayOffset;
	public float overlayVerticalSpacing;

	private List<LevelEntry> levelEntries;
	private List<AttemptEntry> attemptEntries;

	public void AddLevelEntry(int levelId, string levelName, string levelDesigner, string levelMetadata) {
		LevelEntry levelEntry = Instantiate(levelEntryPrefab, rootTransform);
		levelEntry.Initialize(levelId, levelName, levelDesigner, levelMetadata);
		levelEntry.transform.localPosition = new Vector3(offset.x, offset.y + verticalSpacing * levelEntries.Count, 0.0f);

		levelEntries.Add(levelEntry);
		
		rootTransform.sizeDelta = new Vector2(
			rootTransform.sizeDelta.x, Mathf.Abs(offset.y + verticalSpacing * levelEntries.Count));
	}

	public void AddAttemptEntry(int levelId, string levelName, string attemptDesigner, 
		string levelMetadata, string attemptMetadata,
		float avgSpeed, float budgetReq, float score) {
		AttemptEntry attemptEntry = Instantiate(attemptEntryPrefab, overlayRootTransform);
		attemptEntry.Initialize(levelId, levelName, attemptDesigner, levelMetadata, attemptMetadata, avgSpeed, budgetReq, score);
		attemptEntry.transform.localPosition = new Vector3(overlayOffset.x, overlayOffset.y + overlayVerticalSpacing * attemptEntries.Count, 0.0f);

		attemptEntries.Add(attemptEntry);

		overlayRootTransform.sizeDelta = new Vector2(
			overlayRootTransform.sizeDelta.x, Mathf.Abs(overlayOffset.y + overlayVerticalSpacing * attemptEntries.Count));
	}

	public void OpenOverlay() {
		overlay.SetActive(true);
	}

	public void CloseOverlay() {
		overlay.SetActive(false);
	}

	public void ClearLevels() {
		foreach (LevelEntry levelEntry in levelEntries) {
			Destroy(levelEntry.gameObject);
		}

		levelEntries.Clear();
	}

	public void ClearAttempts() {
		foreach (AttemptEntry attemptEntry in attemptEntries) {
			Destroy(attemptEntry.gameObject);
		}
		
		attemptEntries.Clear();
	}

	private void Awake() {
		levelEntries = new List<LevelEntry>();
		attemptEntries = new List<AttemptEntry>();
	}

}
