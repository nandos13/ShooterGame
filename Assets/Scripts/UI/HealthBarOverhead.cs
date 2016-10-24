using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* DESCRIPTION:
 * Controls a health bar UI element. This script goes on the prefab
 * for the health bar itself.
 */

public class HealthBarOverhead : MonoBehaviour {
	
	public Vector3 offset;									// Position offset
	[Range (0.1f, 3.0f)]
	public float scale = 1;									// Scale of the health bar element
	[Range (4.0f, 10.0f)]
	public float transitionSpeed = 10;						// Speed of bar movement when damage is taken

	public bool fadeOut = true;
	[Range (0.01f, 3.0f)]
	public float fadeTime = 3.0f;

	[HideInInspector] public GameObject owner;				// The owner of the health bar
	[HideInInspector] public Health healthComponent;		// The health script being monitored

	public RawImage innerFill;								// The inner fill of the health bar

	private bool fading = false;

	void Update () 
	{
		if (owner)
			transform.position = owner.transform.position + offset;

		if (innerFill)
		{
			if (healthComponent)
			{
				// Handle health bar size based on damage
				Vector3 newScale = innerFill.rectTransform.localScale;
				newScale.x = Mathf.Lerp (newScale.x, healthComponent.CurrentHealthDec, transitionSpeed * Time.deltaTime);
				newScale.x = Mathf.Clamp (newScale.x, 0, 1);
				innerFill.rectTransform.localScale = newScale;

				// Check if the object is now dead
				if (!healthComponent.Alive)
				{
					if (!fading) 
						StartCoroutine (destroyOnDeath());
				}
			}
			else
			{
				Vector3 newScale = innerFill.rectTransform.localScale;
				newScale.x = Mathf.Lerp (newScale.x, 0, transitionSpeed * Time.deltaTime);
				innerFill.rectTransform.localScale = newScale;
			}
		}

		// Rotate the canvas to always face the player
		Camera mainCam = Camera.main;
		Vector3 angleToPlayer = transform.position - mainCam.transform.position;
		transform.rotation = Quaternion.LookRotation (angleToPlayer, Vector3.up);
	}

	public void SetScale (Vector3 sc)
	{
		transform.localScale = sc;
	}

	IEnumerator destroyOnDeath ()
	{
		fading = true;

		if (fadeOut && fadeTime > 0)
		{
			CanvasGroup cg = GetComponent<CanvasGroup>();
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
			{
				cg.alpha = 1 - t;
				yield return null;
			}
			Destroy (this.gameObject);
		}
		else
		{
			yield return new WaitForSeconds (fadeTime);
			Destroy (this.gameObject);
		}
	}
}
