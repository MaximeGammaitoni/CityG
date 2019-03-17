using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiManager  {

    public CanvasScaler canvasScaler;

    public enum ScreenEnum {ScreenHome, ScreenCard, ScreenClan, ScreenLoading}
    public Dictionary<ScreenEnum, ScreenAbstract> screens;

    public enum JaugeEnum {JaugeXp, JaugeGas, JaugeGasStats, Security, Relationship, GreenGas, WellBeing}
    public Dictionary<JaugeEnum, JaugeAbstract> jauges;

    public enum UiElementEnum {UserProfil, LevelSkins, BonusAmount}
    public Dictionary<UiElementEnum, UiElementAbstract> uiElements;

    public enum RewardEnum {Slot1, Slot2, Slot3, Slot4 }
    public Dictionary<RewardEnum, Reward> rewards;

    public enum PopUpEnum {Ranking, Conection, Option, CreateClan, ClanInfo, EndCampaign}
    public Dictionary<PopUpEnum, AbstractPopUp> popUps;

    #region Clan Management
    public MessagesManager messagesManager;
    public ClanListManager clanListManager;
    public ClanInfoButton myClanInfoButton;
    #endregion
    #region Bonus Management
    public WaitingListBonusesManager waitingListBonusesManager;
    #endregion
    public Hiddener hiddener;

    public delegate void JaugeEvent();
    public static event JaugeEvent OnUpdateJauge;

    public delegate void UserProfilEvent();
    public static event UserProfilEvent OnUpdateUserProfil;

    public delegate void LevelSkinEvent();
    public static event LevelSkinEvent OnUpdateLevelSkin;

    public delegate void LoadingScreenEvent();
    public static event LoadingScreenEvent OnLoadingIsComplete;
    public static event LoadingScreenEvent OnLoadingBegin;

    //Call after instantiate in GameManager
    public void Init()
    {
        canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
        screens = new Dictionary<ScreenEnum, ScreenAbstract>();
        jauges = new Dictionary<JaugeEnum, JaugeAbstract>();
        uiElements = new Dictionary<UiElementEnum, UiElementAbstract>();
        rewards = new Dictionary<RewardEnum, Reward>();
        popUps = new Dictionary<PopUpEnum, AbstractPopUp>();

        //Resources.FindObjectsOfTypeAll can find GameObject which are disabled ! Useful :)
        ClanManager.OnIJoinedAClan += (string key, string value)=>{ UpdateUserProfilTrigger(); };
        ClanManager.OnILeavedAClan += (string key, string value) => { UpdateUserProfilTrigger(); };

        myClanInfoButton = GameObject.Find("ClanInfo_Button").GetComponent<ClanInfoButton>();
        myClanInfoButton.clanID = GameManager.singleton.clanManager.GET_ClanID();

        ClanManager.OnIJoinedAClan += myClanInfoButton.HandleOnIJoinedAClan;
        ClanManager.OnILeavedAClan += myClanInfoButton.HandleOnILeavedAClan;

        messagesManager = Resources.FindObjectsOfTypeAll<MessagesManager>()[0];
        messagesManager.Init();

        clanListManager = GameObject.Find("ClansList_View").GetComponent<ClanListManager>();
        clanListManager.Init();

        messagesManager.CreateMessages(GameManager.singleton.clanManager.GET_messages());
        messagesManager.StartListening();

        waitingListBonusesManager = GameObject.Find("WaitingListBonuses_View").GetComponent<WaitingListBonusesManager>();
        waitingListBonusesManager.Initialize();
        //waitingListBonusesManager.InstantiateWaitingList();
        //waitingListBonusesManager.StartUpdateRoutine();
        
        GameManager.singleton.StartCouroutineInGameManager(clanListManager.AskForClanList());
        ////////
        screens.Add(ScreenEnum.ScreenHome, new ScreenHome("ScreenHome"));
        screens.Add(ScreenEnum.ScreenCard, new ScreenCard("ScreenCard"));
        screens.Add(ScreenEnum.ScreenClan, new ScreenClan("ScreenClan"));
        screens.Add(ScreenEnum.ScreenLoading, new ScreenLoading("LoadingSplashScreen"));

        //HOME SCREEN JAUGE
        jauges.Add(JaugeEnum.JaugeXp, new JaugeXp("XPGauge_Viewport"));
        jauges.Add(JaugeEnum.JaugeGas, new JaugeGas("GreenGas_Viewport"));
        jauges.Add(JaugeEnum.JaugeGasStats, new JaugeGas("StatsGreenGas_Viewport"));

        //CARD SCREEN JAUGE
        //jauges.Add(JaugeEnum.GreenGas, new JaugePoints("GreenGasJauge"));
        //jauges.Add(JaugeEnum.Security, new JaugePoints("SecurityJauge"));
        //jauges.Add(JaugeEnum.WellBeing, new JaugePoints("WellBeingJauge"));
        //jauges.Add(JaugeEnum.Relationship, new JaugePoints("CustomerRelationshipJauge"));

        uiElements.Add(UiElementEnum.UserProfil, new UserProfil("UserProfile_Viewport"));
        uiElements.Add(UiElementEnum.LevelSkins, new LevelsSkin("LevelSkin_Viewport"));
        uiElements.Add(UiElementEnum.BonusAmount, new BonusAmount("Bonus_Viewport"));

        /*rewards.Add(RewardEnum.Slot1, new Reward("Slot1", 1));
        rewards.Add(RewardEnum.Slot2, new Reward("Slot2", 2));
        rewards.Add(RewardEnum.Slot3, new Reward("Slot3", 3));
        rewards.Add(RewardEnum.Slot4, new Reward("Slot4", 4));*/

        popUps.Add(PopUpEnum.Ranking, new RankingPopUp("RankingPopUp"));
        popUps.Add(PopUpEnum.Conection, new ConectionPopUp("ConectionPopUp"));
        popUps.Add(PopUpEnum.Option, new OptionPopUp("OptionPopUp"));
        popUps.Add(PopUpEnum.CreateClan, new CreateClanPopUp("PopUpCreateClan", "ScreenClan"));
        popUps.Add(PopUpEnum.ClanInfo, new ClanInfoPopUp("PopUpClanInfo", "ScreenClan"));

        hiddener = new Hiddener("Hiddener");
        GameManager.SystemOnInit += TriggerAll;
        GameManager.SystemOnInit += LoadingIsComplete;
    }

    public void ClearListeners()
    {
        OnLoadingBegin = null;
        OnLoadingIsComplete = null;
        OnUpdateJauge = null;
        OnUpdateLevelSkin = null;
        OnUpdateUserProfil = null;
    }

    public void UpdateJaugeTrigger()
    {
        if (OnUpdateJauge != null)
        {
            OnUpdateJauge();
        }
    }

    public void UpdateLevelSkinTrigger()
    {
        if (OnUpdateLevelSkin != null)
        {
            OnUpdateLevelSkin();
        }
    }

    public void UpdateUserProfilTrigger()
    {
        if (OnUpdateUserProfil != null)
        {
            OnUpdateUserProfil();
        }
    }

    public void LoadingIsComplete()
    {
        if (OnLoadingIsComplete != null)
        {
            OnLoadingIsComplete();
        }
    }

    public void LoadingBegin()
    {
        if (OnLoadingBegin != null)
        {
            OnLoadingBegin();
        }
    }

    public void TriggerAll()
    {
        UpdateLevelSkinTrigger();
        UpdateJaugeTrigger();
        UpdateUserProfilTrigger();
    }
}
