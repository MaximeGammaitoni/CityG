using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpPopUpButton : MonoBehaviour {
    protected PopUpWinManager popUpWinManager;
	// Use this for initialization
	void Start () {
        popUpWinManager = GameObject.Find("CityGazManager").GetComponent<PopUpWinManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CloseLevelPopUp()
    {
        popUpWinManager.CloseLevelUpPopUp();
    }
}
