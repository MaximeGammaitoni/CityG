using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseQuitPopUp : MonoBehaviour {

	// Use this for initialization
	public void Close () {
        GameManager.singleton.inGameUiManager.popUpQuit.Close();
    }
}
