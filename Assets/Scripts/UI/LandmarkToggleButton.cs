public class LandmarkToggleButton : ToggleButton {

	public int landmarkAssetId;

	public override void OnActivated() {
		base.OnActivated();

		SiteManager.Instance.landmarkManager.landmarkBuilder.InitializeBuilder(landmarkAssetId);
	}

}
