using UnityEngine;
using System.Collections;
using UnityEngine.UI;   // UI data types such as images and buttons

/*  ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////


    //-- need any help, ask me: damian
            Script lets us have images/text box open with a different animations

            3 types :   Width to Height ~ opens image width 1st then drops open height
                        Height to Width ~ opens image Height 1st then scrolls across to width
                        Height and Width ~ opens from achors Height and width at same time and expands open       

    ~ set anchors to topleft of textbox, so textbox animates from there to textbox size.
    ~ set "Open Style" values.
    ~ (helpful tip) Width and Height values in rect transform should match "Open Box Size" values.
    ~ drag TextBox Object into UI settings "Text Box", drag Text Object into UI settings "Text".
    ~ click '+' in On Click() - drag ToolTip object, set it to ToolTip.StartOpen()


    ////////////////////////////////////////////////////////////////////////////////////////////////////////
*/  ////////////////////////////////////////////////////////////////////////////////////////////////////////




public class Tooltip : MonoBehaviour
{
    [System.Serializable]   // serializes the class, keeps it tidy in unity
    public class AnimationSettings  // contains settings for UI and how it changes during runtime
    {
        public enum OpenStyle { WidthToHeight, HeightToWidth, HeightAndWidth };
        [Tooltip("Type of animation when opening")]
        public OpenStyle openStyle; // how the tool tip opens
        [Range(0.0f, 100.0f), Tooltip("Speed of textbox when it opens")]    // adds slider in unity
        public float widthSmooth = 0.1f, heightSmooth = 0.1f;   // speed of textbox when it opens
        [Range(0.0f, 100.0f), Tooltip("How fast text will appear")]     // adds slider in unity
        public float textSmooth = 0.1f; // how fast text will appear
        [HideInInspector]   // only works for the next line of code, hide the boolean values
        public bool widthOpen = false, heightOpen = false; // opens one at a time, we need to know when each is done opening
        public void Initialize()    // initialize is called when amination starts over
        {
            widthOpen = false;
            heightOpen = false;
        }
    }

    [System.Serializable]
    public class UIsettings
    {
        [Tooltip("Drag and drop child's image of button")]
        public Image textBox;   // will contain text content
        [Tooltip("Drag and drop child's text of image")]
        public Text text;   // tooltip message
        public Vector2 initialBoxSize = new Vector2(0.25f, 0.25f);    // size of textbox without any scaling
        [Tooltip("Opened box size should match Rect Transform's value")]
        public Vector2 openedBoxSize = new Vector2(400, 400);
        public float snapToSizeDistance = 0.2f; //distance from current size to target size before current is snapped to target
        [Range(0.0f, 100.0f), Tooltip("Time of text image stays open")]     // adds slider in unity
        public float lifeSpan = 1;
        // these next varibles dont need to be seen from the inspector, but are public
        [HideInInspector]
        public bool opening = false;    // starts when opening is false
        [HideInInspector]
        public Color textColor;     // text colour reference
        [HideInInspector]
        public Color textBoxColor;
        [HideInInspector]
        public RectTransform textBoxRect;   // lets us modify the size of textbox
        [HideInInspector]
        public Vector2 currentSize;     // text box sizing
        public void Initialize()    // initialize our values for animation cycle
        {
            textBoxRect = textBox.GetComponent<RectTransform>();
            textBoxRect.sizeDelta = initialBoxSize;     // is a vector2 size of the rect with respec to its anchor
            currentSize = textBoxRect.sizeDelta;
            opening = false;
            textColor = text.color;
            textColor.a = 0;
            text.color = textColor;
            textBoxColor = textBox.color;
            textBoxColor.a = 1;
            textBox.color = textBoxColor;
            textBox.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }
    }

    public AnimationSettings animateSettings = new AnimationSettings();
    public UIsettings uiSettings = new UIsettings();
    float lifeTimer = 0;

    void Start()
    {
        animateSettings.Initialize();
        uiSettings.Initialize();
    }

    public void StartOpen()     // method that calls when button is clicked
    {
        uiSettings.opening = true;
        uiSettings.textBox.gameObject.SetActive(true);
        uiSettings.text.gameObject.SetActive(true);
    }

    void Update()
    {
        if (uiSettings.opening)
        {
            OpenToolTip();
            if (animateSettings.widthOpen && animateSettings.heightOpen)
            {
                lifeTimer += Time.deltaTime;
                if (lifeTimer > uiSettings.lifeSpan)
                {
                    FadeToolTipOut();
                }
                else
                {
                    FadeTextIn();
                }
            }
        }
    }

    void OpenToolTip()
    {
        // what open style to be used
        switch (animateSettings.openStyle)
        {
            case AnimationSettings.OpenStyle.WidthToHeight:
                OpenWidthToHeight();
                break;
            case AnimationSettings.OpenStyle.HeightToWidth:
                OpenHeightToWidth();
                break;
            case AnimationSettings.OpenStyle.HeightAndWidth:
                OpenHeightAndWidth();
                break;
            default:
                Debug.LogError("Open animation is not set");
                break;
        }
        uiSettings.textBoxRect.sizeDelta = uiSettings.currentSize;
    }

    void OpenWidthToHeight()
    {
        if (!animateSettings.widthOpen)
        {
            OpenWidth();
        }
        else
        {
            OpenHeight();
        }
    }

    void OpenHeightToWidth()
    {
        if (!animateSettings.heightOpen)
        {
            OpenHeight();
        }
        else
        {
            OpenWidth();
        }
    }

    void OpenHeightAndWidth()
    {
        if (!animateSettings.widthOpen)
        {
            OpenWidth();
        }
        if (!animateSettings.heightOpen)
        {
            OpenHeight();
        }
    }

    void OpenWidth()
    {
        uiSettings.currentSize.x = Mathf.Lerp(uiSettings.currentSize.x, uiSettings.openedBoxSize.x, animateSettings.widthSmooth * Time.deltaTime);

        if (Mathf.Abs(uiSettings.currentSize.x - uiSettings.openedBoxSize.x) < uiSettings.snapToSizeDistance)
        {
            uiSettings.currentSize.x = uiSettings.openedBoxSize.x;
            animateSettings.widthOpen = true;
        }
    }

    void OpenHeight()
    {
        uiSettings.currentSize.y = Mathf.Lerp(uiSettings.currentSize.y, uiSettings.openedBoxSize.y, animateSettings.heightSmooth * Time.deltaTime);

        if (Mathf.Abs(uiSettings.currentSize.y - uiSettings.openedBoxSize.y) < uiSettings.snapToSizeDistance)
        {
            uiSettings.currentSize.y = uiSettings.openedBoxSize.y;
            animateSettings.heightOpen = true;
        }
    }

    void FadeTextIn()
    {
        uiSettings.textColor.a = Mathf.Lerp(uiSettings.textColor.a, 1, animateSettings.textSmooth * Time.deltaTime);
        uiSettings.text.color = uiSettings.textColor;
    }

    void FadeToolTipOut()
    {
        uiSettings.textColor.a = Mathf.Lerp(uiSettings.textColor.a, 0, animateSettings.textSmooth * Time.deltaTime);
        uiSettings.text.color = uiSettings.textColor;
        uiSettings.textBoxColor.a = Mathf.Lerp(uiSettings.textBoxColor.a, 0, animateSettings.textSmooth * Time.deltaTime);
        uiSettings.textBox.color = uiSettings.textBoxColor;
        if (uiSettings.textBoxColor.a <= 0.01)   // Animation finished and reinitialize
        {
            uiSettings.opening = false;
            animateSettings.Initialize();
            uiSettings.Initialize();
            lifeTimer = 0;
        }
    }
}

