using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour {
    public GameObject gameManagerPrefab;
    void Start () {
        if (GameManager.singleton != null)
        {
            GameManager.singleton.StartGameManager();
        }
        else
        {
            GameObject gameMangerGameObject = Instantiate(gameManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            GameManager.singleton.StartGameManager();
        }
        
    }
}
