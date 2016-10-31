using UnityEngine;
using System.Collections;

//-- cursor lock for resume after pause.
//-- Drag script to 'Game Manager' element 'On Resume'

public class UnlockCursor : MBAction
{

    public override void Execute()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
