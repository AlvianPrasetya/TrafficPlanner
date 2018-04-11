public class ExitRoad : Road, ICollectible {

	public AccessPointInfographic accessPointInfographic;

	public override Grid EndGrid {
		get {
			return base.EndGrid;
		}

		set {
			base.EndGrid = value;

			Enlist();
		}
	}

	public void Enlist() {
		SiteManager.Instance.trafficManager.trafficBuilder.AddExitRoad(this);
	}

	public void Delist() {
		SiteManager.Instance.trafficManager.RemoveExitRoad(this);
		SiteManager.Instance.trafficManager.trafficBuilder.RemoveExitRoad(this);
	}

	public override void Demolish() {
		Delist();

		base.Demolish();
	}

}
