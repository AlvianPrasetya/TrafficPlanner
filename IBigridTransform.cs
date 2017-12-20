public interface IBigridTransform {

	Grid StartGrid {
		get;
		set;
	}

	Grid EndGrid {
		get;
		set;
	}

	void Split(Grid intermediateGrid, out IBigridTransform head, out IBigridTransform tail);

}
