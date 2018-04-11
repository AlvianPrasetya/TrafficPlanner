public interface IValidatable {

	bool IsValid {
		get;
		set;
	}

	void Validate();

}
