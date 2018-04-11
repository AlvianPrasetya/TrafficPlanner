public class TransactableRoad : Road, ICollectible, ITransactable {

	public float costPerUnit;

	private float cost;

	public override Grid StartGrid {
		get {
			return base.StartGrid;
		}

		set {
			base.StartGrid = value;

			Cost = roadLength * costPerUnit;
		}
	}

	public override Grid EndGrid {
		get {
			return base.EndGrid;
		}

		set {
			base.EndGrid = value;

			Cost = roadLength * costPerUnit;
		}
	}

	public override void Demolish() {
		Cost = 0.0f;
		Delist();

		base.Demolish();
	}

	public float Cost {
		get {
			return cost;
		}

		set {
			BudgetManager.Instance.AddToBudget(value - cost);

			cost = value;
		}
	}

	public void Enlist() {
		SiteManager.Instance.roadManager.AddRoad(this);
	}

	public void Delist() {
		SiteManager.Instance.roadManager.RemoveRoad(this);
	}

	protected override void Awake() {
		base.Awake();

		Enlist();
		Cost = 0.0f;
	}

}
