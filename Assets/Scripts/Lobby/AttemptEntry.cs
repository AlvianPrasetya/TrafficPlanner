using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AttemptEntry : MonoBehaviour {

	public Text attemptDesigner;
	public Text avgSpeed;
	public Text budgetReq;
	public Text score;

	private int levelId;
	private string levelName;
	private string levelMetadata;
	private string attemptMetadata;

	public void Initialize(int levelId, string levelName, string attemptDesigner, 
		string levelMetadata, string attemptMetadata, float avgSpeed, float budgetReq, float score) {
		this.levelId = levelId;
		this.levelName = levelName;
		this.attemptDesigner.text = attemptDesigner;
		this.levelMetadata = levelMetadata;
		this.attemptMetadata = attemptMetadata;
		this.avgSpeed.text = avgSpeed.ToString("0.0%");
		this.budgetReq.text = string.Format("$ {0:0.00}", budgetReq);
		this.score.text = score.ToString("0.000");
	}

	public void OnClick() {
		// Load game scene with this entry's metadata
		SessionManager.Instance.InitializeLevel(levelId, levelMetadata, attemptMetadata);

		SceneManager.LoadScene(SceneUtils.LEVEL_ATTEMPT);
	}

}
