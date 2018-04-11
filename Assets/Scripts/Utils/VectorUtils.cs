using UnityEngine;

public class VectorUtils : MonoBehaviour {
	
	/**
	 * This method checks whether 2 line segments described by their end coordinates are intersecting each other 
	 * at an intersection point.
	 * Returns whether the 2 line segments intersect and their intersection coordinate.
	 */
	public static bool AreLineSegmentsIntersecting(Vector3 aStart, Vector3 aEnd, Vector3 bStart, Vector3 bEnd, out Vector3 intersection) {
		Vector3 da = aEnd - aStart;
		Vector3 db = bEnd - bStart;
		Vector3 dAStartBStart = bStart - aStart;
		Vector3 dBStartAStart = aStart - bStart;
		Vector3 dBStartAEnd = aEnd - bStart;
		Vector3 dAStartBEnd = bEnd - aStart;

		// Case 1: Parallel lines
		if (da.normalized == db.normalized) {
			// Collinear intersecting ends with line b as an extension of line a
			if (aEnd == bStart) {
				intersection = bStart;
				return true;
			}

			// Collinear intersecting ends with line a as an extension of line b
			if (bEnd == aStart) {
				intersection = aStart;
				return true;
			}

			// Collinear overlapping lines
			if (dBStartAEnd.normalized == dAStartBEnd.normalized) {
				// Collinear overlapping with line b completely enclosing line a
				if (dBStartAEnd.sqrMagnitude > da.sqrMagnitude && dAStartBEnd.sqrMagnitude > da.sqrMagnitude) {
					intersection = aStart;
					return true;
				}

				// Collinear overlapping with line a completely enclosing line b
				if (dBStartAEnd.sqrMagnitude > db.sqrMagnitude && dAStartBEnd.sqrMagnitude > db.sqrMagnitude) {
					intersection = bStart;
					return true;
				}

				// Collinear overlapping with line b as an extension of line a
				if (dAStartBEnd.sqrMagnitude > dBStartAEnd.sqrMagnitude) {
					intersection = bStart;
					return true;
				}

				// Collinear overlapping with line a as an extension of line b
				if (dBStartAEnd.sqrMagnitude > dAStartBEnd.sqrMagnitude) {
					intersection = aStart;
					return true;
				}
			}

			// Lines are parallel but not collinear
			intersection = Vector3.zero;
			return false;
		}

		// Case 2: Non-parallel coplanar lines
		if (Vector3.Dot(dAStartBStart, Vector3.Cross(da, db)) == 0.0f) {
			// The lines are coplanar, intersection is possible here
			float relativeLengthFromAStart = Vector3.Dot(
				Vector3.Cross(dAStartBStart, db), Vector3.Cross(da, db)) / Vector3.SqrMagnitude(Vector3.Cross(da, db));
			float relativeLengthFromBStart = Vector3.Dot(
				Vector3.Cross(dBStartAStart, da), Vector3.Cross(db, da)) / Vector3.SqrMagnitude(Vector3.Cross(db, da));
			if (relativeLengthFromAStart >= 0.0f && relativeLengthFromAStart <= 1.0f 
				&& relativeLengthFromBStart >= 0.0f && relativeLengthFromBStart <= 1.0f) {
				intersection = aStart + da * relativeLengthFromAStart;
				return true;
			}
		}

		// Lines are not parallel or intersecting on a common plane
		intersection = Vector3.zero;
		return false;
	}

}
