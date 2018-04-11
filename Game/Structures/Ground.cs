using UnityEngine;

public class Ground : MonoBehaviour {

	public Renderer groundRenderer;

	public Vector3 Scale {
		get {
			return transform.localScale;
		}

		set {
			transform.localScale = value;
			groundRenderer.material.mainTextureScale = new Vector2(value.x, value.z);
		}
	}

}
