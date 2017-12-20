using UnityEngine;

public class Ground : MonoBehaviour {

	public new Renderer renderer;

	public Vector3 Scale {
		get {
			return transform.localScale;
		}

		set {
			transform.localScale = value;
			renderer.material.mainTextureScale = new Vector2(value.x, value.z);
		}
	}

}
