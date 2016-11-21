using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class btnBomb : upgradeBtnBase {

	private Button btn;
	public Color purchasedColour;
	public Color expensiveColour;
	public List<MBAction> onPurchase = new List<MBAction> ();

	void Start () 
	{
		btn = GetComponent<Button>();
	}

	void Update () 
	{
		// Change button appearance based on whether the upgrade is available, already bought, or unaffordable
		if (btn)
		{
			if (GameStatistics._atBomber)
			{
				// Upgrade already purchased
				btn.interactable = false;
				ColorBlock temp = btn.colors;
				temp.disabledColor = purchasedColour;
				btn.colors = temp;
			}
			else if (GameStatistics.Score >= cost && checkCriteria())
			{
				// Upgrade can be purchased
				btn.interactable = true;
			}
			else
			{
				// Upgrade is too expensive
				btn.interactable = false;
				ColorBlock temp = btn.colors;
				temp.disabledColor = expensiveColour;
				btn.colors = temp;
			}
		}

		base.UpdateVisuals(GameStatistics._atBomber);
	}

	private bool checkCriteria ()
	{
		// Checks that previously required upgrades have already been obtained
		if (GameStatistics._atLaser)
			return true;
		return false;
	}

	public override void Execute ()
	{
		// Handle what happens when the button is clicked
		bool purchased = GameStatistics.unlockAttachment(GUN_ATTACHMENTS.Bomber, cost);

		if (purchased)
		{
			// Run purchase scripts
			foreach (MBAction action in onPurchase)
			{
				if (action)
					action.Execute();
			}
		}
	}
}
