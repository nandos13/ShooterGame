using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Updates the player's weapons to reflect the attachments that have been unlocked
 * through the shop system.
 */

public class ApplyAttachments : MBAction {

	public BulletWeapon MainAuto;
	public GameObject Shotty;
	public GameObject FlameThrower;
	public GameObject Laser;
	public GameObject Minigun;
	public GameObject Bomber;
	public GameObject RPG;

	public override void Execute ()
	{
		if (MainAuto)
		{
			if (GameStatistics._atMainAuto)
				MainAuto.fMode = FIRE_MODE.Auto;
			else
				MainAuto.fMode = FIRE_MODE.SemiAuto;
		}

		if (Shotty)
			Shotty.SetActive(GameStatistics._atShotty);

		if (FlameThrower)
			FlameThrower.SetActive(GameStatistics._atFlameThrower);

		if (Laser)
			Laser.SetActive(GameStatistics._atLaser);

		if (Minigun)
			Minigun.SetActive(GameStatistics._atMinigun);

		if (Bomber)
			Bomber.SetActive(GameStatistics._atBomber);

		if (RPG)
			RPG.SetActive(GameStatistics._atRPG);
	}
}
