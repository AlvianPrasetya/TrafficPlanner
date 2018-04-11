public interface IObservable<T, U> {
	
	void AddObserver(Observer<T, U> observer);

	void RemoveObserver(Observer<T, U> observer);

	void ClearObservers();

	void NotifyObservers(U data);

}
