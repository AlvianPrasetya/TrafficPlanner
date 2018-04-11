using System.Collections.Generic;

public class Path {

	private LinkedList<Road> roads;
	private float pathLength;

	public Path() {
		roads = new LinkedList<Road>();
		pathLength = 0;
	}

	public Path(Path otherPath) {
		roads = new LinkedList<Road>(otherPath.Roads);
		pathLength = otherPath.pathLength;
	}

	public LinkedList<Road> Roads {
		get {
			return roads;
		}
	}

	public void Append(Road road) {
		roads.AddLast(road);
		pathLength += road.RoadLength;
	}

	public void Truncate() {
		pathLength -= roads.Last.Value.RoadLength;
		roads.RemoveLast();
	}

	public float PathLength {
		get {
			return pathLength;
		}
	}

}
