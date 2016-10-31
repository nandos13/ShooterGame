using UnityEngine;
using System.Collections;

public class QuitGame : MBAction
{
    public override void Execute()
    {
        Application.Quit();
    }
}
