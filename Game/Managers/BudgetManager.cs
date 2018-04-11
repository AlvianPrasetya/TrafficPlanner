using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BudgetManager : MonoBehaviour {

	public Text budgetText;
	public float initialBudget;

	private float budget;

	private Coroutine updateDisplayedBudgetCoroutine;

	private static BudgetManager instance;

	public static BudgetManager Instance {
		get {
			return instance;
		}
	}

	public float Budget {
		get {
			return budget;
		}
	}

	public void AddToBudget(float addValue) {
		budget += addValue;

		if (updateDisplayedBudgetCoroutine != null) {
			StopCoroutine(updateDisplayedBudgetCoroutine);
		}
		updateDisplayedBudgetCoroutine = StartCoroutine(UpdateDisplayedBudget());
	}

	public void SubFromBudget(float subValue) {
		budget -= subValue;

		if (updateDisplayedBudgetCoroutine != null) {
			StopCoroutine(updateDisplayedBudgetCoroutine);
		}
		updateDisplayedBudgetCoroutine = StartCoroutine(UpdateDisplayedBudget());
	}

	private void Awake() {
		instance = this;

		budget = DisplayedBudget = initialBudget;
	}

	private float DisplayedBudget {
		get {
			return float.Parse(budgetText.text.TrimStart(new char[] { '$', ' ' }));
		}

		set {
			budgetText.text = string.Format("$ {0:0.00}", value);
		}
	}

	private IEnumerator UpdateDisplayedBudget() {
		while (DisplayedBudget != budget) {
			float diff = budget - DisplayedBudget;
			float delta = diff * Time.deltaTime;

			if (Mathf.Abs(delta) < 1.0f) {
				DisplayedBudget += diff;
			} else {
				DisplayedBudget += delta;
			}
			yield return null;
		}

		updateDisplayedBudgetCoroutine = null;
	}

}
