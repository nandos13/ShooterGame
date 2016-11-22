using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConsoleDisableOptionBtn : MBAction {

	public Button button;

	public override void Execute ()
	{
		if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
		{
			if (button)
			{
				button.interactable = false;
			}
		}
	}
}
