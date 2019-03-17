using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameUiManager {
    #region PROTECTED PROPERTIES
    protected UnityEngine.Events.UnityAction skipButtonFadeInFunc;
    protected UnityEngine.Events.UnityAction skipButtonFadeOutFunc;
    #endregion
    #region PUBLIC PROPERTIES
    public Text movesText;
    public Text scoreText;
    public JaugeXp gaugeXp;
    public GameObject skipButton;
    public PopUpLooseManager popUpLooseManager;
    public PopUpWinManager popUpWinManager;
    public PopUpQuit popUpQuit;
    public EndCampaignPopUp popUpEndCampaign;
    public ScreenLoading screenLoading;
    public GameObject boostersPanel;
    public GazTipsGateway gazTipsGateway;

    #endregion
    #region PROTECTED METHODS
    void Update()
    {
        if (SessionInfo.current != null)
        {
            if (SessionInfo.current.GetMovesCount() != 0)
            {
                movesText.text = SessionInfo.current.GetMovesCount().ToString();
            }
        }
    }
    #endregion
    #region PUBLIC METHODS
    public void SubscribeToEvents()
    {
        skipButtonFadeInFunc = () => 
        {
            skipButton.transform.GetChildByPath("Button").GetComponent<DebugButton>().FadeIn("Right");
            Project.onAllTargetsIsReached.RemoveListener(this.skipButtonFadeInFunc);
        };
        skipButtonFadeOutFunc = () => 
        {
            skipButton.transform.GetChildByPath("Button").GetComponent<DebugButton>().FadeOut("Right");
        };
        Project.onLevelStart.AddListener(() => 
        {
            GameObject.FindObjectsOfType<BoosterButton>().ForEach((BoosterButton x) => { x.Refresh(); });
        });
        Project.onLevelStart.AddListener(UpdateUIElementsSize);
        Project.onLevelStart.AddListener(UpdateMoveCount);
        Project.onLevelStart.AddListener(popUpWinManager.GetComponent<PopUpWinManager>().LoadAndAssignGazTips);
        Project.onLevelFailed.AddListener(popUpLooseManager.Open);
        Project.onLevelFailed.AddListener(skipButtonFadeOutFunc);
        Project.onLevelComplete.AddListener(popUpWinManager.Open);
        Project.onLevelComplete.AddListener(skipButtonFadeOutFunc);
        Project.onPlayingModeChanged.AddListener((PlayingMode pm)=> { UpdateMoveCount(); });
        Project.onAllTargetsIsReached.AddListener(skipButtonFadeInFunc);
        Project.onScoreChanged.AddListener(UpdateScore);

    }
    public void UpdateScore()
    {
        if (SessionInfo.current != null && scoreText!=null)
        {
            scoreText.text = "Score : "+SessionInfo.current.GetScore().ToString();
            popUpWinManager.UpdateScore(SessionInfo.current.GetScore());
        }
    }
    public void UpdateMoveCount()
    {
        if (movesText != null)
        {
            movesText.text = SessionInfo.current.GetMovesCount().ToString();
        }
    }
    public void AnimateXpGained()
    {
        //GameManager.singleton.StartCouroutineInGameManager(gaugeXp.AnimateToReach(), "AnimateToReach");
    }
    public void UpdateXPGauge()
    {
        gaugeXp.UpdateGaugeFill();
    }
    public void UpdateUIElementsSize()
    {
        boostersPanel = GameObject.Find("MBoosterPanel").gameObject;

        if (boostersPanel != null)
        {
            float l_width = boostersPanel.GetComponent<RectTransform>().rect.width;
            float l_height = boostersPanel.GetComponent<RectTransform>().rect.height;
            boostersPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2( l_height * 0.75f, l_height * 0.75f);
        }
    }
    #endregion
    #region CONSTRUCTORS
    public InGameUiManager()
    {
        GameObject l_cityGazManager = GameObject.Find("CityGazManager").gameObject;
        popUpLooseManager = l_cityGazManager.GetComponent<PopUpLooseManager>();
        popUpWinManager = l_cityGazManager.GetComponent<PopUpWinManager>();
        popUpQuit = new PopUpQuit("Quit_PopUp");
        popUpEndCampaign = new EndCampaignPopUp("EndCampaignPopUp");
        screenLoading = new ScreenLoading("LoadingSplashScreen");
        movesText = GameObject.Find("Moves").transform.Find("Text").GetComponent<Text>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        skipButton = GameObject.Find("Skip_Button").gameObject;
        gazTipsGateway = new GazTipsGateway(GameManager.singleton.fireBaseClient);
        popUpWinManager.UpdateXPGauge();
        GameManager.SystemOnInit += () => { screenLoading.Close();};
    }
    #endregion
}
