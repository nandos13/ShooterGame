using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * 
 */

public class DamageFlashHandler : MonoBehaviour {

	private Health playerHealth;
	public Canvas flasherCanvas;

	public HEALTH_IDENTIFIER_MODE mode = HEALTH_IDENTIFIER_MODE.Flash;
	[Range (0.0f, 1.0f)]
	public float maxOpacity = 0.5f;
	[Range (0.01f, 1.5f)]
	public float fadeTime = 0.3f;

	private CanvasGroup cg;
	private IEnumerator flasherCo;
	private float lastHealth;

	void Start () 
	{
		playerHealth = GetComponent<Health>();

		if (!playerHealth)
			Debug.LogWarning("Player Health not set on DamageFlashHandler.");

		if (playerHealth)
			lastHealth = playerHealth.CurrentHealth;

		Debug.Log(lastHealth);
		
		if (flasherCanvas)
		{
			cg = flasherCanvas.GetComponent<CanvasGroup>();
			if (cg)
				cg.alpha = 0;
		}

		flasherCo = flashCanvas();
	}

	void Update () 
	{
		if (cg && playerHealth)
		{
			if (mode == HEALTH_IDENTIFIER_MODE.Flash)
			{
				if (playerHealth.CurrentHealth < lastHealth)
				{
					// Update health
					lastHealth = playerHealth.CurrentHealth;

					// Start the flash
					StopAllCoroutines();
					StartCoroutine(flashCanvas());
				}
			}
			else if (mode == HEALTH_IDENTIFIER_MODE.Opacity)
			{
				if (cg)
				{
					cg.alpha = Mathf.Lerp (cg.alpha, ((1 - playerHealth.CurrentHealthDec) * maxOpacity), fadeTime);
					cg.alpha = Mathf.Clamp (cg.alpha, 0, maxOpacity);
				}
			}
		}
	}

	private IEnumerator flashCanvas ()
	{
		/* Set opacity then fade to 0 alpha */
		if (cg)
		{
			cg.alpha = maxOpacity;
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
			{
				cg.alpha = maxOpacity - (t * maxOpacity);
				//cg.alpha = 1 - t;
				yield return null;
			}
		}
	}
}
