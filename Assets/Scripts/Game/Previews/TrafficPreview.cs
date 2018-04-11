public class TrafficPreview : TrafficInfographic, IValidatable {

	private bool valid;

	public bool IsValid {
		get {
			return valid;
		}

		set {
			valid = value;
		}
	}

	public void Validate() {
		if (StartGrid == null || EndGrid == null) {
			IsValid = false;
			return;
		}

		if (StartGrid == EndGrid) {
			IsValid = false;
			return;
		}

		IsValid = true;
	}

	public override Grid StartGrid {
		get {
			return base.StartGrid;
		}

		set {
			base.StartGrid = value;

			Validate();
		}
	}

	public override Grid EndGrid {
		get {
			return base.EndGrid;
		}

		set {
			base.EndGrid = value;
			
			Validate();
		}
	}

}
