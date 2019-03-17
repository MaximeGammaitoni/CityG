using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseTipsButton : MonoBehaviour {


    protected PopUpWinManager popUpWinManager;

    private void Start()
    {
        popUpWinManager = GameObject.Find("CityGazManager").GetComponent<PopUpWinManager>();
    }
    public void OnClick()
    {
        popUpWinManager.CloseTips();
    }
}
