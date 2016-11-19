using UnityEngine;
using System.Collections;

public static class LevelOrder {

    private static int currentGameLevel = 0;
    public static void SetNextLevel()
    {
        if (currentGameLevel == levelOrder.Length - 1)
            currentGameLevel = 0;
        else
            currentGameLevel++;
    }

    private static string[] levelOrder = { "Main_Menu", "Level_0_Tutorial", "Level_1_Desert", "Level_2_Forest", "Level_3_Ice", "Level_4_Boss", "Shop_Scene", "Credit_Scene", "Death_Scene" };
	//-- {" main menu" -> "tutorial" -> "desert" -> "forest" -> "ice" -> "boss" -> "shop" -> "credits" -> "deathscreen"};
    public static string NextLevel
    {
        get {
            // Figure out which level is next and return the string
            int length = levelOrder.Length;
            if (currentGameLevel == length - 1)
                return levelOrder[0];
            else
                return levelOrder[currentGameLevel + 1];
        }
    }
    public static string levelIndex(int ind)
    {
        if (ind < levelOrder.Length && ind > 0)
            return levelOrder[ind];
        else
            return levelOrder[0];
    }
	public static void PlayerDeathBackLevel ()
	{
		/* When the player dies, they are taken back to the shop menu. However the shop
		 * menu will call the next level, so the level needs to be taken back by one
		 * so the current level will reload.
		 */
		if (currentGameLevel != 0)
			currentGameLevel--;
	}
	public static void SetCurrentLevel (int i)
	{
		if (i < 0)
			currentGameLevel = 0;
		else if (i >= levelOrder.Length)
			currentGameLevel = levelOrder.Length - 1;
		else
			currentGameLevel = i;
	}
}
