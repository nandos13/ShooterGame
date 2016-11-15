using UnityEngine;
using System.Collections;

//-- cursor lock for resume after pause.
//-- Drag script to 'Game Manager' element 'On Resume'

public class LockCursor : MBAction {

    public override void Execute ()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
