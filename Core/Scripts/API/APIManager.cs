using System.Collections;
using System.IO;
using Serilog;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Plugins.Core.PathCore;
using dreamcube.unity.Core.Scripts.API;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Category;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Activity;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.Stores;
using dreamcube.unity.Core.Scripts.Util;
using Screen = dreamcube.unity.Core.Scripts.API.ContentAPI.Screen.Screen;
using UniRx;

//[ExecuteAlways]
public class APIManager : MonoBehaviour
{
    public bool getSuccess { get; set; }
    public bool postSuccess { get; set; }

    private GeneralSettings _settings;

    [SerializeField] private bool useApi;
    [SerializeField] private bool continuousPolling;
    [SerializeField] private float GCMPollingInterval = 0.1f;

    private bool _isInitialized;
    private bool _bIsQuitting;

    private void Awake()
    {
        ConfigManager.LoadingComplete.Subscribe(x =>
        {
            if (x)
            {
                Initialize();
            }
        });
    }

    private void Initialize()
    {
        Log.Debug($"Initializing API manager");

        _settings = ConfigManager.Instance.generalSettings;
        continuousPolling = _settings.ContinuousPolling;
        useApi = _settings.UseAPI;

        if (!_settings.UseAPI)
        {
            Log.Warning($"API services are disabled, make sure you have the desired settings in your configuration file");
            return;
        }

        LoadContentData();
        StartCoroutine(GetGameStates());
        EnablePolling(continuousPolling);
        EventManager.Instance.StartListening(EventStrings.EventOnPitchTriggeredScreenStateChange, EventHandler);
        _isInitialized = true;
    }

    private void OnDisable()
    {
        EnablePolling(false);
    }

    private void EnablePolling(bool bPoll)
    {
        if (bPoll)
            InvokeRepeating(nameof(ContinuousPoll), 2, GCMPollingInterval);
        else
            CancelInvoke(nameof(ContinuousPoll));
    }

    private void ContinuousPoll()
    {
        StartCoroutine(GetGameStates());
    }


    private void OnDestroy()
    {
        _bIsQuitting = true;
        EnablePolling(false);

        if (EventManager.Instance != null)
            EventManager.Instance.StopListening(EventStrings.EventOnPitchTriggeredScreenStateChange, EventHandler);
    }

    private void OnApplicationQuit()
    {
        _bIsQuitting = true;
        EnablePolling(false);
        if (EventManager.Instance != null)
            EventManager.Instance.StopListening(EventStrings.EventOnPitchTriggeredScreenStateChange, EventHandler);
    }

    private void OnValidate()
    {
        if (_bIsQuitting)
            return;

        EnablePolling(continuousPolling);
        //if (!_isInitialized && useApi)
        //    Initialize();
    }

    private void Update()
    {

        if (!StoreUtil.IsDebug)
            return;


        if (Input.GetKeyDown(KeyCode.R))
            Log.Debug($"GenerateEnumTest : {GenerateFakeAPIData.GenerateEnumTest()}");


        if (Input.GetKeyDown(KeyCode.C))
            LoadContentData();

        if (Input.GetKeyDown(KeyCode.G))
            GetGameStates();

        if (Input.GetKeyDown(KeyCode.P))
            PostAll();

        if (Input.GetKeyDown(KeyCode.F))
        {
            SetFakeData(false);
            PostAll();

            //test scoring API
            //PostData(DerivedSettings.ContentServiceScoringUrl, GenerateFakeAPIData.GenerateFakePlayerScoreData(), CallbackOnPostSuccess, CallbackOnFail);

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            //GetAllScores();
            PostData(DerivedSettings.ContentServiceScoringUrl + $"/{_settings.DreamCube}", GenerateFakeAPIData.GenerateFakePlayerScoreData(), CallbackOnPostSuccess, CallbackOnPostFail);
        }

    }

    private void PostAll()
    {
        //PostData(DerrivedSettings.DreamcubeStateDataUrl, DreamCubeSessionDataStore.GetState(), CallbackOnPostSuccess, CallbackOnFail);
        //PostData(DerrivedSettings.GameRoundDataUrl, GameRoundDataStore.GetRoundData(), CallbackOnPostSuccess, CallbackOnFail);
        PostData(DerivedSettings.DreamcubeStateDataUrl + $"/{_settings.DreamCube}", DreamCubeSessionDataStore.GetState(), CallbackOnPostSuccess, CallbackOnPostFail);
        PostData(DerivedSettings.GameRoundDataUrl + $"/{_settings.DreamCube}", GameRoundDataStore.GetRoundData(), CallbackOnPostSuccess, CallbackOnPostFail);
    }

    private static void SetFakeData(bool setIdol)
    {
        DreamCubeSessionDataStore.SetState(GenerateFakeAPIData.GenerateFakeDreamCubeSessionData(setIdol));

        var fakeRoundData = GenerateFakeAPIData.GenerateFakeGameRoundData();
        fakeRoundData.CurrentPlayerData = DreamCubeSessionDataStore.GetState().PlayerDataSet.FirstOrDefault();

        var fakeActivity = GenerateFakeAPIData.GenerateFakeActivity();
        fakeRoundData.CurrentActivityID = fakeActivity.ID;

        GameRoundDataStore.SetRoundData(fakeRoundData);

    }

    private void PostScreenStateUpdate()
    {
        var ScreenStateData = new DreamCubeScreenStateData()
        {
            CurrentDCState = DreamCubeSessionDataStore.CurrentDCScreenState.Value,
            DreamCubeID = ConfigManager.Instance.generalSettings.DreamCube
        };

        if (DreamCubeSessionDataStore.CurrentDCScreenState.Value == DreamCubeScreenStates.DC_ACTIVITY_SELECTION)
        {
            // we reset the score after when each round starts
            GameRoundDataStore.SetScore(0);
        }

        if (DreamCubeSessionDataStore.CurrentDCScreenState.Value == DreamCubeScreenStates.DC_ACTIVITY_SCORE)
        {
            // submit final score
            var currentPlayer = GameRoundDataStore.GetRoundData().CurrentPlayerData;
            Log.Debug($"Final score is {GameRoundDataStore.CurrentScore}");
            PostPlayerScore(currentPlayer, GameRoundDataStore.CurrentScore.Value);
            Invoke(nameof(GetAllScores), 1);
        }

        // this is possibly the wrong thing to do for all screen changes
        PostData(DerivedSettings.DreamcubeStateDataUrl + $"/{_settings.DreamCube}", ScreenStateData,
            CallbackOnPostSuccess, CallbackOnPostFail);
    }

    private void PostPlayerScore(PlayerData playerData, int score)
    {

        var scoreData = new ScoreData()
        {
            DreamCubeID = _settings.DreamCube,
            PlayerID = playerData.PlayerID,
            PlayerName = playerData.PlayerName,
            SessionID = DreamCubeSessionDataStore.GetState().SessionID,
            Score = score
        };

        PostData(DerivedSettings.ContentServiceScoringUrl + $"/{_settings.DreamCube}" , scoreData, CallbackOnPostSuccess, CallbackOnPostFail);
    }

    IEnumerator GetGameStates()
    {
        getSuccess = false;
        GetGenericData(DerivedSettings.GameRoundDataUrl + $"/{_settings.DreamCube}", _settings.ApiGrdStateName, CallbackOnGetGenericSuccess,
            CallbackOnGetFail);

        while (!getSuccess)
        {
            yield return new WaitForEndOfFrame();
        }

        GetGenericData(DerivedSettings.DreamcubeStateDataUrl + $"/{_settings.DreamCube}", _settings.ApiGcmStateName, CallbackOnGetGenericSuccess,
            CallbackOnGetFail);
    }


    private void GetAllScores()
    {
        GetGenericData(DerivedSettings.ContentServiceScoringUrl + $"/{_settings.DreamCube}", _settings.ApiScoringSuffix, CallbackOnGetGenericSuccess,
            CallbackOnGetFail);
    }

    private void LoadContentData()
    {
        GetGenericData(DerivedSettings.ContentServiceCategoriesUrl, _settings.ApiContentCategoriesName, CallbackOnGetGenericSuccess,
            CallbackOnGetFail);

        GetGenericData(DerivedSettings.ContentServiceActivitiesUrl, _settings.ApiContentActivitiesName, CallbackOnGetGenericSuccess,
            CallbackOnGetFail);

        GetGenericData(DerivedSettings.ContentServiceScreensUrl, _settings.ApiContentScreensName, CallbackOnGetGenericSuccess,
            CallbackOnGetFail);

        // testing prefab loading
    }




    public void GetGenericData(string URL, string type, UnityAction<string, string> callbackOnSuccess, UnityAction<string, string> callbackOnFail)
    {
        Log.Verbose($"{nameof(GetGenericData)} URL: {URL} type: {type}");
        ServerCommunication.Instance.SendRequest(URL, type, callbackOnSuccess, callbackOnFail);
    }

    public void PostData<T>(string uri, T data, UnityAction<string> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        Log.Debug($"{nameof(PostData)} {data}");
        ServerCommunication.Instance.PostRequest(uri, data, CallbackOnPostSuccess, callbackOnFail);
    }

    private void CallbackOnPostSuccess(string successMessage)
    {
        postSuccess = true;
        Log.Debug($"{nameof(CallbackOnPostSuccess)} {successMessage}");
    }

    private void CallbackOnGetGenericSuccess(string data, string type)
    {
        getSuccess = true;
        ProcessGetSuccess(data, type);
        Log.Verbose($"{nameof(CallbackOnGetGenericSuccess)} {data} {type}");

    }

    private void ProcessGetFail(string type)
    {
        if (type == _settings.ApiContentCategoriesName || type == _settings.ApiContentActivitiesName ||
            type == _settings.ApiContentScreensName)
        {
            Log.Debug($"{nameof(ProcessGetFail)} loading {type} from local data");
            var data = Extensions.LoadStringFromFile($"{_settings.CMSDataFolder}{type}.json");
            ProcessGetSuccess(data, type);
        }
    }


    public void ProcessGetSuccess(string data, string type)
    {
        Log.Verbose($"{nameof(ProcessGetSuccess)} type: {type}\n Data {data}");

        if (data == "{}")
        {
            Log.Verbose($"{nameof(ProcessGetSuccess)} type: {type}\n Data {data}");
            return;
        }

        var backupFileName = $"{_settings.CMSDataFolder}{type}.json";
        //Log.Debug($"Saving content data to {backupFileName}");


        if (type == _settings.ApiGrdStateName) ProcessGRD(data);
        else if (type == _settings.ApiGcmStateName) ProcessGCM(data);
        else if (type == _settings.ApiContentCategoriesName) ProcessCategories(data, backupFileName);
        else if (type == _settings.ApiContentActivitiesName) ProcessActivities(data, backupFileName);
        else if (type == _settings.ApiContentScreensName) ProcessScreens(data, backupFileName);
        else if (type == _settings.ApiScoringSuffix) ProcessScores(data);
    }

 

    private void CallbackOnGetFail(string errorMessage, string type)
    {
        getSuccess = false;
        Log.Warning($"API request of type {type} Failed {errorMessage}");

        if (type == _settings.ApiContentCategoriesName) ProcessGetFail(type);
        else if (type == _settings.ApiContentActivitiesName) ProcessGetFail(type);
        else if (type == _settings.ApiContentScreensName) ProcessGetFail(type);
    }


    private void CallbackOnPostFail(string errorMessage)
    {
        postSuccess = false;
        Log.Warning($"API Post Failed: {errorMessage}");

    }

    private void ProcessGCM(string data)
    {
        Log.Verbose($"{nameof(ProcessGCM)} {data}");
        DreamCubeSessionData sessionData = JsonUtility.FromJson<DreamCubeSessionData>(data);

        // if the skip round button is pressed on the bench, load the leaderboard
        var currentScreen = DreamCubeSessionDataStore.CurrentDCScreenState.Value;
        if (sessionData.CurrentDCState == DreamCubeScreenStates.DC_ACTIVITY_SCORE && currentScreen != DreamCubeScreenStates.DC_ACTIVITY_SCORE )
            GetAllScores();

        // GCM is not allowed to change anything during intro state, not allowed to change into gameplay state
        if (currentScreen == DreamCubeScreenStates.DC_ACTIVITY_INTRO ||
            sessionData.CurrentDCState == DreamCubeScreenStates.DC_ACTIVITY_GAMEPLAY)
        {
            Log.Verbose($"<color=red>GCM is not allowed to change anything during intro state, not allowed to change into gamePlay state</color>");
            return;
        }

        if (currentScreen == DreamCubeScreenStates.DC_ACTIVITY_GAMEPLAY &&
            sessionData.CurrentDCState == DreamCubeScreenStates.DC_ACTIVITY_INTRO)
        {
            Log.Verbose($"<color=red>going back from gamePlay to intro is not allowed</color>");
            return;
        }

        DreamCubeSessionDataStore.SetState(sessionData);
    }

    //private static void ProcessGCMs(string data)
    //{
    //    Log.Verbose($"{nameof(ProcessGCMs)} {data}");
    //    var GCMs = JsonHelper.FromJson<DreamCubeSessionData>(data);
    //    var sessionData = GCMs.Where(x => x.DreamCubeID == ConfigManager.Instance.generalSettings.DreamCube).ToList();
    //    DreamCubeSessionDataStore.SetState(sessionData.FirstOrDefault());
    //}

    private static void ProcessGRD(string data)
    {
        Log.Verbose($"{nameof(ProcessGRD)} {data}");
        GameRoundData newData = JsonUtility.FromJson<GameRoundData>(data);
        GameRoundDataStore.SetRoundData(newData);

    }

    private static void ProcessScores(string data)
    {
        Log.Debug($"{nameof(ProcessScores)} {data}");
        var scores = JsonHelper.FromJson<ScoreData>(data);
        GameRoundDataStore.SetAllScore(scores);
    }

    private static void ProcessCategories(string data, string filename ="" )
    {
        Log.Debug($"{nameof(ProcessCategories)} filename: {filename} \n {data} ");
        CategoryDataStore.Categories = JsonHelper.FromJson<Category>(data);

        if ( !string.IsNullOrEmpty(filename) && CategoryDataStore.Categories.Any() )
            Extensions.SaveStringToFile(data, filename, useStreamingAssets:false);
    }


    private static void ProcessActivities(string data, string filename = "")
    {
        ActivityDataStore.Activities = JsonHelper.FromJson<Activity>(data);
        Log.Debug($"{nameof(ProcessActivities)} {ActivityDataStore.Activities.Count}");

        if (!string.IsNullOrEmpty(filename) && ActivityDataStore.Activities.Any())
            Extensions.SaveStringToFile(data, filename, useStreamingAssets: false);
    }

    private static void ProcessScreens(string data, string filename = "")
    {
        List<Screen> screens = JsonHelper.FromJson<Screen>(data);
        ScreenDataStore.Screens = screens;

        if (!string.IsNullOrEmpty(filename) && ScreenDataStore.Screens.Any())
            Extensions.SaveStringToFile(data, filename, useStreamingAssets: false);

        Log.Debug($"{nameof(ProcessScreens)} {screens.Count}");
        Log.Debug($"{nameof(ProcessScreens)} {screens.FirstOrDefault().Media.Video.en}");
    }



    private void EventHandler(string theEvent, string msg, GameObject sender, DataModelBase data)
    {
        if (theEvent == EventStrings.EventOnPitchTriggeredScreenStateChange)
        {
            // update the screenState on the API
            if (!_settings.UseAPI) return;

            PostScreenStateUpdate();
            Log.Debug($"<color=orange>{nameof(EventHandler)} {theEvent}</color>");
        }
    }
}

