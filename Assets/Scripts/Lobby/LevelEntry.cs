using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEntry : MonoBehaviour {

	public Text levelName;
	public Text levelDesigner;

	private int levelId;
	private string levelMetadata;

	public void Initialize(int levelId, string levelName, string levelDesigner, string levelMetadata) {
		this.levelId = levelId;
		this.levelName.text = levelName;
		this.levelDesigner.text = levelDesigner;
		this.levelMetadata = levelMetadata;
	}

	public void OnClick() {
		// Load game scene with this entry's metadata
		SessionManager.Instance.InitializeLevel(levelId, levelMetadata, null);

		SceneManager.LoadScene(SceneUtils.LEVEL_ATTEMPT);
	}

	public void OnBrowseAttempts() {
		LobbyManager.Instance.FetchAttempts(levelId);
	}

}
