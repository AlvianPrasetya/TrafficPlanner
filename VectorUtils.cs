using UnityEngine;

public class VectorUtils : MonoBehaviour {
	
	public static bool AreLinesIntersecting(Vector3 aStart, Vector3 aEnd, Vector3 bStart, Vector3 bEnd, out Vector3 intersection) {
		Vector3 da = aEnd - aStart;
		Vector3 db = bEnd - bStart;
		Vector3 dc = bStart - aStart;
		Vector3 dd = aStart - bStart;

		if (Vector3.Dot(dc, Vector3.Cross(da, db)) == 0.0f) {
			// The lines are coplanar, intersection is possible here
			float relativeLengthFromAStart = Vector3.Dot(
				Vector3.Cross(dc, db), Vector3.Cross(da, db)) / Vector3.SqrMagnitude(Vector3.Cross(da, db));
			float relativeLengthFromBStart = Vector3.Dot(
				Vector3.Cross(dd, da), Vector3.Cross(db, da)) / Vector3.SqrMagnitude(Vector3.Cross(db, da));
			if (relativeLengthFromAStart >= 0.0f && relativeLengthFromAStart <= 1.0f 
				&& relativeLengthFromBStart >= 0.0f && relativeLengthFromBStart <= 1.0f) {
				intersection = aStart + da * relativeLengthFromAStart;
				return true;
			}
		}

		// Lines are not coplanar or relativeLengthFromAStart is not within range, no intersection
		intersection = Vector3.zero;
		return false;
	}

}
