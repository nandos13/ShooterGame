using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class upgradeBtnBase : MBAction {

	public Text lockPriceText;
	public Text unlockedText;
	public Image lockImage;
	public string upgradeName = "";
	public int cost;

	protected void UpdateVisuals (bool unlocked, bool unlockable)
	{
		if (lockPriceText)
		{
			lockPriceText.text = cost.ToString();
			if (!unlocked && !unlockable)
				lockPriceText.enabled = true;
			else
				lockPriceText.enabled = false;
		}

		if (unlockedText)
		{
			if (!unlocked)
			{
				if (!unlockable)
					unlockedText.enabled = false;
				else
				{
					unlockedText.text = "$" + cost.ToString() + " - " + upgradeName;
					unlockedText.enabled = true;
				}
			}
			else
			{
				unlockedText.text = upgradeName;
				unlockedText.enabled = true;
			}
		}

		if (lockImage)
		{
			if (!unlocked && !unlockable)
				lockImage.enabled = true;
			else
				lockImage.enabled = false;
		}
	}
}
