using UnityEngine;
using System.Collections;

public class CoroutineUtils {

	public static IEnumerator ScaleWithDuration(Transform transform, Vector3 targetScale, float duration) {
		float relativeScale = 0.0f;
		while (relativeScale < 1.0f) {
			relativeScale = Mathf.Clamp(relativeScale + Time.deltaTime / duration, 0.0f, 1.0f);

			transform.localScale = relativeScale * targetScale;
			yield return null;
		}
	}

}
