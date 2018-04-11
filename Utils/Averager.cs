public class Averager {

	private float average;
	private int numDataPoints;

	public Averager() {
		Reset();
	}

	public float Average {
		get {
			return average;
		}
	}

	public void AddDataPoint(float value) {
		average = (average * numDataPoints + value) / (numDataPoints + 1);
		numDataPoints++;
	}

	public void Reset() {
		average = 0.0f;
		numDataPoints = 0;
	}

}
