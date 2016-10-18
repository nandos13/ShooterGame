using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* DESCRIPTION:
 * Flashes a specified canvas when executed. Intended to broadcast
 * when the player takes damage
 */

public class FlashCanvasOnScreen : MBAction {

	public Canvas canvas;
	private CanvasGroup cg;
	[Range (0.5f, 2.0f)]
	public float fadeTime = 0.5f;

	private IEnumerator fader;

	public void Start ()
	{
		fader = fadeAway();
		cg = canvas.GetComponent<CanvasGroup>();
	}

	public override void Execute () 
	{
		if (canvas)
		{
			StartCoroutine(fader);
		}
	}

	protected IEnumerator fadeAway ()
	{
		if (cg)
		{
			cg.alpha = 1;
			//for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
			//{
			//	cg.alpha = Mathf.Lerp(cg.alpha, 0, fadeTime);
			//	yield return null;
			//}
			//while (cg.alpha > 0)
			//{
			//	//cg.alpha = Mathf.Lerp(cg.alpha, 0, fadeTime);
			//	//cg.alpha -= (fadeTime / Time.deltaTime);
			//	yield return null;
			//}
		}
	}
}
