using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{

    public Reward[] slots;
    // Use this for initialization
    public void Init()
    {
        slots = new Reward[] {
            GameManager.singleton.uiManager.rewards[UiManager.RewardEnum.Slot1],
            GameManager.singleton.uiManager.rewards[UiManager.RewardEnum.Slot2],
            GameManager.singleton.uiManager.rewards[UiManager.RewardEnum.Slot3],
            GameManager.singleton.uiManager.rewards[UiManager.RewardEnum.Slot4]
        };
        SetAllRewards();
    }

    public void SetAllRewards()
    {
        slots[0].SetNewReward(1000, "Usine 1");
        slots[1].SetNewReward(4000, "Usine 2");
        slots[2].SetNewReward(800, "Usine 3");
        //slots[4].SetNewReward(800, "Usine 4");
    } 
}
