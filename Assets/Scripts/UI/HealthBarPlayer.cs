using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* DESCRIPTION:
 * Controls the UI health bar displaying the player's health.
 */

public class HealthBarPlayer : MonoBehaviour {

	public Image bar;
	public Text text;

	[Range (4.0f, 100.0f)]
	public float transitionSpeed = 10;

	private Health healthComponent;

	void Start () 
	{
		GameObject pl = (GameObject.Find("Player"));

		if (pl)
			healthComponent = pl.GetComponent<Health>();

		if (!healthComponent)
			Debug.LogWarning("HealthBarPlayer script was unable to find the Player's health script. The UI will not show the health bar properly.");

		if (!bar)
			Debug.LogWarning("HealthBarPlayer script has no specified health bar image. Please attach the UI health bar to the script.");

		if (!text)
			Debug.LogWarning("HealthBarPlayer script has no specified text element. Text will not update to reflect current health.");
	}

	void Update () 
	{
		if (healthComponent)
		{
			if (bar)
			{
				// Lerp to the new health value
				bar.fillAmount = Mathf.Lerp (bar.fillAmount, healthComponent.CurrentHealthDec, transitionSpeed * Time.deltaTime);

				// Clamp the value
				bar.fillAmount = Mathf.Clamp (bar.fillAmount, 0, 100);
			}

			if (text)
			{
				// Update text
				text.text = (((int)healthComponent.CurrentHealthPct).ToString());
			}
		}
	}
}
