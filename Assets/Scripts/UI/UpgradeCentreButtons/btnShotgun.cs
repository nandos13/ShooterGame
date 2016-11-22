using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class btnShotgun : upgradeBtnBase {

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
			if (GameStatistics._atShotty)
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

		base.UpdateVisuals(GameStatistics._atShotty, checkCriteria());
	}

	private bool checkCriteria ()
	{
		// Checks that previously required upgrades have already been obtained
		if (GameStatistics._atMainAuto)
			return true;
		return false;
	}

	public override void Execute ()
	{
		// Handle what happens when the button is clicked
		bool purchased = GameStatistics.unlockAttachment(GUN_ATTACHMENTS.Shotty, cost);

		if (purchased)
		{
			// Run purchase scripts
			foreach (MBAction action in onPurchase)
			{
				if (action)
					action.Execute();
			}

			// Select next button
			if (btn.navigation.selectOnDown)
				btn.navigation.selectOnDown.Select();
			else if (btn.navigation.selectOnRight)
				btn.navigation.selectOnRight.Select();
		}
	}
}
