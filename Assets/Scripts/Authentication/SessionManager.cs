using UnityEngine;

public class SessionManager : MonoBehaviour {

	public string host;

	private static SessionManager instance;
	private string username;
	private string sessionId;

	private int levelId;
	private LevelMetadata levelMetadata;
	private LevelMetadata attemptMetadata;

	public static SessionManager Instance {
		get {
			return instance;
		}
	}

	public string Username {
		get {
			return username;
		}
	}

	public string SessionId {
		get {
			return sessionId;
		}
	}
	
	public int LevelId {
		get {
			return levelId;
		}
	}

	public LevelMetadata LevelMetadata {
		get {
			return levelMetadata;
		}
	}

	public LevelMetadata AttemptMetadata {
		get {
			return attemptMetadata;
		}
	}

	public void InitializeSession(string username, string sessionId) {
		this.username = username;
		this.sessionId = sessionId;
	}

	public void InitializeLevel(int levelId, string levelMetadata, string attemptMetadata) {
		this.levelId = levelId;
		this.levelMetadata = new LevelMetadata(levelMetadata);
		this.attemptMetadata = new LevelMetadata(attemptMetadata);
	}

	private void Awake() {
		DontDestroyOnLoad(gameObject);

		// Another persisting copy already exists, destroy this instance
		if (FindObjectsOfType(GetType()).Length > 1) {
			Destroy(gameObject);
		}

		instance = this;
	}

}
