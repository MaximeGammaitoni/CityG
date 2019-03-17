using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNewGame : MonoBehaviour {

    public bool debugging;
    public int debugLevel;

    protected int levelToGo;

    private void Awake()
    {
        GameManager.SystemOnInit += CallSearchForCurrentLevel;
    }

    void CallSearchForCurrentLevel()
    {
        SearchForCurrentLevel();
        GameManager.SystemOnInit -= CallSearchForCurrentLevel;
    }
    void SearchForCurrentLevel()
    {
        #if UNITY_EDITOR
        if (debugging)
        {
            levelToGo = (debugLevel > 0) ? debugLevel : 1;
        }
        else
        {
            levelToGo = (GameManager.singleton.playerManager.GetPlayerCurrentLevel() != -1) ? GameManager.singleton.playerManager.GetPlayerCurrentLevel() : 1;
        }
        #else
        levelToGo = (GameManager.singleton.playerManager.GetPlayerCurrentLevel() != -1) ? GameManager.singleton.playerManager.GetPlayerCurrentLevel() : 1;
        #endif
        Debug.Log("Level to select is :" + levelToGo);
        LevelAssistant.SelectDesign(levelToGo);
    }
}
