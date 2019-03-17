using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour {

    private PopUpWinManager popUpWinManager;

    private void Start()
    {
        popUpWinManager = GameObject.Find("CityGazManager").GetComponent<PopUpWinManager>();
    }
    public void OnClick()
    {
        popUpWinManager.Close();
    }
}
