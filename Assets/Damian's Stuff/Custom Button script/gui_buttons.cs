using UnityEngine;
using System.Collections;


// -- Drag script on to object, drag GUIskin in script



[ExecuteInEditMode]
public class gui_buttons : MonoBehaviour        // custom buttons editer
{
    public bool debugMode = false;
    public Texture2D buttonImage = null;
    public Rect position = new Rect(15, 15, 150, 150);
    public string title = string.Empty;
    public GUISkin skin = null;

    private void OnGUI()        // Tests the button, change Vector3 values to move cube!
    {
        GUI.skin = skin;
        if (debugMode || Application.isPlaying)
        {
            if (GUI.Button(position, title))
            {
                transform.position += new Vector3(0, 0.0f, 0);
            }
        }
    }
}
