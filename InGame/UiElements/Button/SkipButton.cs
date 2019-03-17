using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour {
    public Button button;
    public RectTransform rectTransform;

    public void SkipAnimation ()
    {
        SessionAssistant.main.StopCurrentSession(false);
    }
}
