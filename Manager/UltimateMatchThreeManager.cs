using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateMatchThreeManager {
    public delegate void E_UMT(string[] p_args);
    public event E_UMT OnGameStart;
    public event E_UMT OnGamePause;
    public event E_UMT OnGameResume;
    public event E_UMT OnGameQuit;
    public event E_UMT OnGoToNextLevel;
    public event E_UMT OnGoToMainMenu;
    public AbstractTutorial firstTimePlayingMatchThreeTutorial;

    public Dictionary<int, int> bonusAvailablesAtLevelStart = new Dictionary<int, int>();
    public Dictionary<int, int> bonusAvailables = new Dictionary<int, int>();
	// Use this for initialization
    public void SubscribeToEvents()
    {
        Project.onLevelComplete.AddListener(() => {
            PlayerPrefs.SetString("firstTimePlayingMatchThree", "false");
            int levelComplete = GameManager.singleton.playerManager.GetPlayerCurrentLevel();
            GameManager.singleton.playerManager.SetPlayerCurrentLevel(levelComplete + 1);
            GameManager.singleton.playerManager.AddPlayerXp(1);
            GameManager.singleton.playerManager.AddPlayerGreenGas(0.1f);
            if(GameManager.singleton.playerManager.playerBonusesManager.GET_WaitingList().Count < 3)
            {
                int l_rng = Random.Range(0, 3);
                Debug.Log("BonusDrop ID:"+l_rng.ToString());
                AbstractBonus l_abstractDrop = CityGazBonus.CreateAbstractBonus(l_rng);
                CityGazBonusTimer l_timerDrop;
                switch (l_abstractDrop.GET_ID())
                {
                    case 0:
                        //Punto
                        l_timerDrop = new CityGazBonusTimer(new System.TimeSpan(2, 0, 0), new System.TimeSpan(2, 0, 0));
                        break;
                    case 1:
                        //Tubes
                        l_timerDrop = new CityGazBonusTimer(new System.TimeSpan(3, 0, 0), new System.TimeSpan(3, 0, 0));
                        break;
                    case 2:
                        //Gaspard
                        l_timerDrop = new CityGazBonusTimer(new System.TimeSpan(4, 0, 0), new System.TimeSpan(4, 0, 0));
                        break;
                    default:
                        l_timerDrop = new CityGazBonusTimer(new System.TimeSpan(4, 0, 0), new System.TimeSpan(4, 0, 0));
                        break;
                }
                CityGazBonusLock l_lockDrop = new CityGazBonusLock(true);
                CityGazBonus l_drop = new CityGazBonus(l_abstractDrop, l_timerDrop, l_lockDrop);
                GameManager.singleton.playerManager.playerBonusesManager.AddToWaitingList(l_drop);
            }
            SaveBonusesUsed();
        });
    }

    public void SaveBonusesUsed()
    {
        int l_difference = 0;
        foreach(KeyValuePair<int,int> current in bonusAvailablesAtLevelStart)
        {
            l_difference = Mathf.Abs(bonusAvailables[current.Key] - current.Value);
            if(l_difference > 0)
            {
                for(int i = 0; i < l_difference; i++)
                {
                    CityGazBonus l_cgb = GameManager.singleton.playerManager.playerBonusesManager.RemoveFromUnlockedList(current.Key);
                    Debug.Log("Removing Bonus : ID:"+l_cgb.GET_Bonus().GET_ID()+" KEY_ID:" + l_cgb.GET_Bonus().GET_KEYID() + " From Local & SERVER Unlocked List, SUCCESSFUL");
                }
            }
        }

    }

	public UltimateMatchThreeManager() {
		string userId = PlayerPrefs.GetString("currentUserId", null);
		//GameManager.singleton.fireBaseClient.BuildPlayerEntity(userId);
        string l_displayTutorial = PlayerPrefs.GetString("firstTimePlayingMatchThree","true");
        if(l_displayTutorial == "true") { 
            firstTimePlayingMatchThreeTutorial = new FirstPlayMatchThree();
            Project.onLevelCreate.AddListener(() => { firstTimePlayingMatchThreeTutorial.StartSequence(0); });
        }
        
        Project.onLevelStart.AddListener(() =>
        {
            //Bonus
            List<CityGazBonus> l_list = GameManager.singleton.playerManager.playerBonusesManager.GET_UnlockedList();
            for(int i = 0; i < 3; i++)
            {
                bonusAvailablesAtLevelStart[i] = 0;
                bonusAvailables[i] = 0;
            }
            foreach (CityGazBonus current in l_list)
            {
                bonusAvailablesAtLevelStart[current.GET_Bonus().GET_ID()] += 1;
                bonusAvailables[current.GET_Bonus().GET_ID()] += 1;
            }
            
        });
        
    }
}