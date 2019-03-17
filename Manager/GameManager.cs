using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager singleton;
    [HideInInspector] public Dictionary<string, IEnumerator> coroutines;
    [HideInInspector] public delegate void GameEventManager();
    [HideInInspector] public static event GameEventManager SystemOnInit;

    [HideInInspector] public static event GameEventManager ApplicationOnQuit;
    [HideInInspector] public static event GameEventManager ApplicationOnPause;
    [HideInInspector] public static event GameEventManager ApplicationOnFocus;

    [HideInInspector] public static event GameEventManager GameUpdate;
    [HideInInspector] public static event GameEventManager GameFixedUpdate;

    public UiManager uiManager;
    public PlayerManager playerManager;
    public ClanManager clanManager;
    public WorldCurrentDateClient worldCurrentDateClient;
    public RewardManager rewardManager;
    public UltimateMatchThreeManager ultimateMatchThreeManager;
    public InGameUiManager inGameUiManager;

    public FireBaseClient fireBaseClient;
    public Text debugText;

    public bool deletePlayersPrefs;
    public void DebugLogToScreen(string p_debugLog)
    {
        GameObject l_go = GameObject.Find("Debug_Text");
        if (l_go)
        {
            Text l_text = l_go.GetComponent<Text>();
            if (l_text)
            {
                l_text.text = p_debugLog;
            }
        }

    }
    public void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != null && singleton != this)
        {
            Debug.LogWarning("singleton:" + singleton.ToString());
            Debug.LogWarning("GameManager Gameobject, OnAwake : Singleton already assigned. Need to destroy this gameobject.");
            Destroy(this);
            return;
        }

        if (deletePlayersPrefs)
        {
            PlayerPrefs.DeleteAll();
            deletePlayersPrefs = false;
        }

        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }
    public void StartGameManager()
    {
        try {
            Debug.Log("GameManager->StartGameManager()->Try, InitServicesWithSceneState()");
            InitServicesWithSceneState();
            DontDestroyOnLoad(this.gameObject);
        }
        catch (Exception e) {
            Debug.LogException(e);
            debugText.text ="DEBUG : "+  e.Message;
        }

    }
    public void OnDisable()
    {
        
        //TODO : disable other game event
    }

    public void InitUnitySystem()
    {
        if (SystemOnInit != null)
        {
            Debug.Log(" GAME MANAGER INIT UNITY SYSTEM");
            SystemOnInit();
        }
    }

    private void Update()
    {
        UpdateForHomeAndBackKey();
        if (GameUpdate != null)
            GameUpdate();
    }

    private void FixedUpdate()
    {
        if (GameFixedUpdate != null)
            GameFixedUpdate();
    }
    public void StartCouroutineInGameManager(IEnumerator routine, string routineName)
    {
        if (coroutines == null)
        {
            coroutines = new Dictionary<string, IEnumerator>();
        }
        if(coroutines!= null && !coroutines.ContainsKey(routineName))
        {
            Coroutine co = StartCoroutine(routine);
            coroutines.Add(routineName, routine);
        }
        else if (coroutines != null && coroutines.ContainsKey(routineName))
        {
            StopCouroutineInGameManager(routineName);
            Coroutine l_co = StartCoroutine(routine);
            coroutines.Add(routineName, routine);
        }
    }
    public void StartCouroutineInGameManager(IEnumerator routine)//Coroutine avec arret automatique du MonoBehavior
    {
        StartCoroutine(routine);
    }
    public void StopCouroutineInGameManager(string coroutineName)
    {
        if (coroutines.ContainsKey(coroutineName))
        {
            StopCoroutine(coroutines[coroutineName]);
            coroutines.Remove(coroutineName);
        }
    }

    void OnApplicationQuit()
    {
        if (ApplicationOnQuit != null)
            ApplicationOnQuit();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (ApplicationOnFocus != null)
                ApplicationOnFocus();
        }
        else
        {
            Debug.Log("Unfocus");
            if (ApplicationOnPause != null)
                ApplicationOnPause();
        }
    }

    void UpdateForHomeAndBackKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //DEBUG
            ApplicationOnPause();
        }
    }

    public void DestroyServices()
    {
        StopAllCoroutines();
        DestroyAllManagers();
        DestroyAllClients();
        DestroyAllListeners();
        coroutines = null;
    }

    private void DestroyAllManagers()
    {
        if (uiManager != null && uiManager.messagesManager != null)
            uiManager.messagesManager.Unload();
        if(uiManager!= null && uiManager.clanListManager != null)
            uiManager.clanListManager.Unload();
        if (uiManager != null)
            uiManager.ClearListeners();
        uiManager = null;
        playerManager = null;
        clanManager = null;
        inGameUiManager = null;
        rewardManager = null;
        ultimateMatchThreeManager = null;
    }
    private void DestroyAllClients()
    {
        worldCurrentDateClient = null;
        if(fireBaseClient!=null)
            fireBaseClient.RemoveAllListeners();
        fireBaseClient = null;
    }

    private void DestroyAllListeners()
    {
        SystemOnInit = null;
        ApplicationOnQuit = null;
        ApplicationOnPause = null;
        ApplicationOnFocus = null;
        GameUpdate = null;
        GameFixedUpdate = null;
    }

    public void InitServicesWithSceneState()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Citygaz":
                Debug.Log("START HOME SCENE");
                StartCouroutineInGameManager(InitCityGazInterfaceServices(), "InitCityGazInterfaceServices");
                break;
            case "Game":
                Debug.Log("START GAME SCENE");
                StartCouroutineInGameManager(InitCityGazGameServices(), "InitCityGazGameServices");
                break;
            case "SplashScreen":
                string l_firstPlay = PlayerPrefs.GetString("FirstTimePlayingCityGaz", "true");

                if (l_firstPlay == "true")
                {
                    PlayerPrefs.SetString("FirstTimePlayingCityGaz", "false");
                    SceneStatic.GoToGameScene();
                    StartCouroutineInGameManager(InitCityGazGameServices(), "InitCityGazGameServices");
                }
                else
                {
                    SceneStatic.GoToMenuScene();
                    StartCouroutineInGameManager(InitCityGazInterfaceServices(), "InitCityGazInterfaceServices");
                }
                break;
            default:
                Debug.LogError("InitServicesWithSceneState-> GetActiveSceneName is not handled !");
                break;
        }
    }

    protected IEnumerator InitCityGazInterfaceServices()
    {
        DebugLogToScreen("");
        string l_debugLog = "Is Waiting until scene is loaded";
        string l_screenLog = "Gazton démarre l'usine !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        yield return new WaitUntil(() => { return SceneManager.GetActiveScene().isLoaded; });
        coroutines = new Dictionary<string, IEnumerator>();
        DebugLogToScreen("");
        l_debugLog = "Init cityGaz interface";
        l_screenLog = "Détermination du plan de route";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        fireBaseClient = new FireBaseClient();
        yield return new WaitUntil(() => { Debug.Log("Waiting FIREBASE RESPONSE..."); return fireBaseClient.fireBaseIsInitialised == true; });
        
        //PLAYER INITIALIZATION
        l_debugLog = "Waiting for Player's service initialization";
        l_screenLog = "Analyse biométrique du Gazier";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        playerManager = new PlayerManager(fireBaseClient);
        playerManager.Init();
        while (playerManager.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.INITIALIZED)
        {
            //l_debugLog += ".";
            //Debug.Log(l_debugLog);
            yield return null;
        }
        l_debugLog = "Player's service initialized";
        l_screenLog = "Scanner rétinien réussi !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //PLAYER INITIALIZED
        //PLAYER LOADING
        l_debugLog = "Waiting for Player's service Loading";
        l_screenLog = "Enfilage de la tenue de Gazier";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        playerManager.Load();
        while (playerManager.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.LOADED)
        {
            //l_debugLog += ".";
            //Debug.Log(l_debugLog);
            yield return null;
        }
        l_debugLog = "Player's service Loaded";
        l_screenLog = "Lacets sérrés, chemise dans le pantalon !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //PLAYER LOADED

        //CLAN INITIALIZATION
        l_debugLog = "Waiting for Clan's service initialization";
        l_screenLog = "Distribution de la feuille de présence...";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        clanManager = new ClanManager(fireBaseClient);
        clanManager.Init();
        while(clanManager.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.INITIALIZED)
        {
            //l_debugLog += ".";
            //Debug.Log(l_debugLog);
            yield return null;
        }
        l_debugLog = "Clan's service Initialized";
        l_screenLog = "Datée, signée, envoyée !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //CLAN INITIALIZED

        //CLAN LOADING
        l_debugLog = "Waiting for Clan's service Loading";
        l_screenLog = "Bonjour aux collègues.";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        clanManager.Load();
        while (clanManager.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.LOADED && clanManager.clanGateway.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.LOADED)
        {
            //l_debugLog += ".";
            //Debug.Log(l_debugLog);
            yield return null;
        }
        l_debugLog = "Clan's service Loaded";
        l_screenLog = "Tout le monde va bien !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //CLAN LOADED
        //UI
        uiManager = new UiManager();
        uiManager.Init();
        uiManager.messagesManager.Init();
        //OTHERS
        worldCurrentDateClient = new WorldCurrentDateClient();

        if (PlayerPrefs.GetString("firstTimeInInterface", "true") == "true")
        {
            FirstTimeInterface l_fti = new FirstTimeInterface();
            SystemOnInit += () => { l_fti.StartSequence(0); };
        }
        else
        {
            List<CityGazBonus> l_listBonus = GameManager.singleton.playerManager.playerBonusesManager.GET_WaitingList();
            bool l_puntoTuto = false;
            bool l_tubesTuto = false;
            bool l_gaspardTuto = false;

            for (int i = 0; i < l_listBonus.Count;i++)
            {
                int l_currentID = l_listBonus[i].GET_Bonus().GET_ID();
                switch (l_currentID)
                {
                    case 0:
                        if (PlayerPrefs.GetString("Tutorial_Bonuses_Punto", "false") == "false")
                            l_puntoTuto = true;
                        break;
                    case 1:
                        if (PlayerPrefs.GetString("Tutorial_Bonuses_Tubes", "false") == "false")
                            l_tubesTuto = true;
                        break;
                    case 2:
                        if (PlayerPrefs.GetString("Tutorial_Bonuses_Gaspard", "false") == "false")
                            l_gaspardTuto = true;
                        break;
                }
            }
            if (l_puntoTuto || l_tubesTuto || l_gaspardTuto)
            {
                BonusesTutorial l_bonusesTutorial = new BonusesTutorial(l_puntoTuto, l_tubesTuto, l_gaspardTuto);
                l_bonusesTutorial.StartSequence(0);
                Debug.LogError("BONUSES TUTORIAL START");
            }
        }

        InitUnitySystem();
        l_debugLog = "[Game manager] Initialized main UI";
        l_screenLog = "Arrivée à la maison...";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
    }

    protected IEnumerator InitCityGazGameServices()
    {
        DebugLogToScreen("");
        string l_debugLog = "Is Waiting until scene is loaded";
        string l_screenLog = "Gazton démarre l'usine !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        yield return new WaitUntil(() => { return SceneManager.GetActiveScene().isLoaded; });
        DebugLogToScreen("");
        coroutines = new Dictionary<string, IEnumerator>();
        fireBaseClient = new FireBaseClient();
        Debug.Log("Is Waiting until firebase is initialised");
        yield return new WaitUntil(() => fireBaseClient.fireBaseIsInitialised == true);
        //PLAYER INITIALIZATION
        l_debugLog = "Waiting for Player's service initialization";
        l_screenLog = "Analyse biométrique du Gazier";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        playerManager = new PlayerManager(fireBaseClient);
        playerManager.Init();
        while (playerManager.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.INITIALIZED)
        {
            //l_debugLog += ".";
            //Debug.Log(l_debugLog);
            yield return null;
        }
        l_debugLog = "Player's service initialized";
        l_screenLog = "Scanner rétinien réussi !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //PLAYER INITIALIZED
        //PLAYER LOADING
        l_debugLog = "Waiting for Player's service Loading";
        l_screenLog = "Enfilage de la tenue de Gazier";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        playerManager.Load();
        while (playerManager.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.LOADED)
        {
            //l_debugLog += ".";
            //Debug.Log(l_debugLog);
            yield return null;
        }
        l_debugLog = "Player's service Loaded";
        l_screenLog = "Lacets sérrés, chemise dans le pantalon !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //PLAYER LOADED

        ultimateMatchThreeManager = new UltimateMatchThreeManager();
        l_debugLog = "ultimateMatchThreeManager created";
        l_screenLog = "Tableau de bord, check !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        inGameUiManager = new InGameUiManager();
        l_debugLog = "InGameUIManager created";
        l_screenLog = "Mise en route de l'éclairage";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        inGameUiManager.gazTipsGateway.Init();
        l_debugLog = "GazTipsGateway Init Begin";
        l_screenLog = "Récupération des règles de sécurité...";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        while (inGameUiManager.gazTipsGateway.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.INITIALIZED)
        {
            yield return null;
        }
        l_debugLog = "GazTipsGateway Initialized";
        l_screenLog = "Evaluation des procédures";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //Gaz tips Gateway Initialiazed
        inGameUiManager.gazTipsGateway.Load();
        l_debugLog = "GazTipsGateway Loading";
        l_screenLog = "Respect des procédures...";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        while (inGameUiManager.gazTipsGateway.currentLifeCycleState != AbstractLifeCycle.LIFECYCLESTATE.LOADED)
        {
            yield return null;
        }
        l_debugLog = "GazTipsGateway Loaded";
        l_screenLog = "Indicateurs au vert !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        //Gaz Tips Gateway Loaded

        
        /*
         * LAUNCH LOAD LEVELS
         * 
         * */
        l_debugLog = "Waiting for LevelDesigns to load...";
        l_screenLog = "Assemblage des tuyaux...";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        LevelAssistant.main.CallLoadLevels();
        yield return new WaitUntil(() => { return LevelAssistant.main.designs!=null && LevelAssistant.main.designs.Count>0; });
        l_debugLog = "LevelDesigns Loaded";
        l_screenLog = "Circuit de production prêt !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        l_debugLog = "Calling InitUnitySystem";
        l_screenLog = "C'est parti !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
        InitUnitySystem();
        l_debugLog = "[Game manager] Initialized Waste Crush";
        l_screenLog = "Usine prête à produire du Gaz Vert !";
        Debug.Log(l_debugLog);
        DebugLogToScreen(l_screenLog);
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
        Debug.Log("Token: " + token.Token);
        GameManager.singleton.playerManager.SetNotificationId(token.Token);
        Debug.Log(GameManager.singleton.playerManager.GetNotificationId());
        Debug.Break();
        if (!PlayerPrefs.HasKey("deviceMessagingId"))
        {
            PlayerPrefs.SetString("deviceMessagingId", token.Token);
            PlayerPrefs.Save();
        }
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
        Debug.Log("Received a new message from: " + e.Message.From);
    }
}