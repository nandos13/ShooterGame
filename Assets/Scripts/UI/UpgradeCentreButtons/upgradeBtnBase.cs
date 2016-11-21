using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class upgradeBtnBase : MBAction {

	public Text priceText;
	public Text unlockedText;
	public Image lockImage;
	public int cost;

	protected void UpdateVisuals (bool unlocked)
	{
		if (priceText)
		{
			priceText.text = cost.ToString();
			if (!unlocked)
				priceText.enabled = true;
			else
				priceText.enabled = false;
		}

		if (unlockedText)
		{
			if (!unlocked)
				unlockedText.enabled = false;
			else
				unlockedText.enabled = true;
		}

		if (lockImage)
		{
			if (!unlocked)
				lockImage.enabled = true;
			else
				lockImage.enabled = false;
		}
	}
}
