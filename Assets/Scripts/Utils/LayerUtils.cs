using UnityEngine;

public class LayerUtils {

	public class Name {
		public static readonly string EMPTY_GRID = "Empty Grid";
		public static readonly string BASE_GRID = "Base Grid";
		public static readonly string OCCUPIED_GRID = "Occupied Grid";
		public static readonly string ROAD = "Road";
		public static readonly string VEHICLE = "Vehicle";
		public static readonly string LANDMARK = "Landmark";
		public static readonly string INFOGRAPHIC = "Infographic";
	}

	public class Id {
		public static readonly int EMPTY_GRID = LayerMask.NameToLayer(Name.EMPTY_GRID);
		public static readonly int BASE_GRID = LayerMask.NameToLayer(Name.BASE_GRID);
		public static readonly int OCCUPIED_GRID = LayerMask.NameToLayer(Name.OCCUPIED_GRID);
		public static readonly int ROAD = LayerMask.NameToLayer(Name.ROAD);
		public static readonly int VEHICLE = LayerMask.NameToLayer(Name.VEHICLE);
		public static readonly int LANDMARK = LayerMask.NameToLayer(Name.LANDMARK);
		public static readonly int INFOGRAPHIC = LayerMask.NameToLayer(Name.INFOGRAPHIC);
	}

	public class Mask {
		public static readonly int EMPTY_GRID = 1 << Id.EMPTY_GRID;
		public static readonly int BASE_GRID = 1 << Id.BASE_GRID;
		public static readonly int OCCUPIED_GRID = 1 << Id.OCCUPIED_GRID;
		public static readonly int ROAD = 1 << Id.ROAD;
		public static readonly int VEHICLE = 1 << Id.VEHICLE;
		public static readonly int LANDMARK = 1 << Id.LANDMARK;
		public static readonly int INFOGRAPHIC = 1 << Id.INFOGRAPHIC;
	}

}
