using UnityEngine;

public class LayerUtils {

	public class Name {
		public static readonly string EMPTY_GRID = "Empty Grid";
		public static readonly string SELECTABLE_GRID = "Selectable Grid";
		public static readonly string OCCUPIED_GRID = "Occupied Grid";
		public static readonly string ROAD = "Road";
		public static readonly string VEHICLE = "Vehicle";
		public static readonly string LANDMARK = "Landmark";
	}

	public class Id {
		public static readonly int EMPTY_GRID = LayerMask.NameToLayer(Name.EMPTY_GRID);
		public static readonly int SELECTABLE_GRID = LayerMask.NameToLayer(Name.SELECTABLE_GRID);
		public static readonly int OCCUPIED_GRID = LayerMask.NameToLayer(Name.OCCUPIED_GRID);
		public static readonly int ROAD = LayerMask.NameToLayer(Name.ROAD);
		public static readonly int VEHICLE = LayerMask.NameToLayer(Name.VEHICLE);
		public static readonly int LANDMARK = LayerMask.NameToLayer(Name.LANDMARK);
	}

	public class Mask {
		public static readonly int EMPTY_GRID = 1 << Id.EMPTY_GRID;
		public static readonly int SELECTABLE_GRID = 1 << Id.SELECTABLE_GRID;
		public static readonly int OCCUPIED_GRID = 1 << Id.OCCUPIED_GRID;
		public static readonly int ROAD = 1 << Id.ROAD;
		public static readonly int VEHICLE = 1 << Id.VEHICLE;
		public static readonly int LANDMARK = 1 << Id.LANDMARK;

		public static readonly int STRUCTURE = ROAD;
	}

}
