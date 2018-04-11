public class ArterialRoad : Road, ICollectible {

	public override Grid EndGrid {
		get {
			return base.EndGrid;
		}

		set {
			base.EndGrid = value;

			Enlist();
		}
	}

	public override void Demolish() {
		Delist();

		base.Demolish();
	}

	public void Enlist() {
		SiteManager.Instance.roadManager.AddArterial(this);
	}

	public void Delist() {
		SiteManager.Instance.roadManager.RemoveArterial(this);
	}

	protected override void Awake() {
		base.Awake();

		Enlist();
	}

}
