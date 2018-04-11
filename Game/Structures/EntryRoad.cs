public class EntryRoad : Road, ICollectible {

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
		SiteManager.Instance.trafficManager.trafficBuilder.AddEntryRoad(this);
	}

	public void Delist() {
		SiteManager.Instance.trafficManager.RemoveEntryRoad(this);
		SiteManager.Instance.trafficManager.trafficBuilder.RemoveEntryRoad(this);
	}

	public override void Demolish() {
		Delist();

		base.Demolish();
	}

}
