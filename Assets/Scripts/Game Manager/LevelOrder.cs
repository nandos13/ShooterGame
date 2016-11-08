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

    private static string[] levelOrder = { "Main_Menu", "testScene", "Credit_Scene" };      //-- {" main menu" -> "tutorial" -> "desert" -> "forest" -> "ice" -> "boss" -> "shop" -> "credits" -> "deathscreen"};
    public static string NextLevel
    {
        get {
            // Figure out which level is next and return the string
            int length = levelOrder.Length;
            if (currentGameLevel == length - 1)
            {
                return levelOrder[0];
            }
            else
            {
                return levelOrder[currentGameLevel + 1];
            }
        }
    }
    public static string levelIndex(int ind)
    {
        if (ind < levelOrder.Length && ind > 0)
            return levelOrder[ind];
        else
            return levelOrder[0];
    }
}
