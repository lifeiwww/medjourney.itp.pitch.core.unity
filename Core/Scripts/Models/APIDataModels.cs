using System;
using System.Collections.Generic;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.API
{


    [Serializable]
    public enum LANGUAGE
    {
        LANGUAGE_ZH = 0,
        LANGUAGE_EN
    }


    [Serializable]
    public static class GCMStates
    {
        public static string GCM_STATE_NONE => nameof(GCM_STATE_NONE);
        public static string GCM_STATE_IDLE => nameof(GCM_STATE_IDLE);
        public static string GCM_STATE_ASSIGNED => nameof(GCM_STATE_ASSIGNED);
        public static string GCM_STATE_UNLOCKED => nameof(GCM_STATE_UNLOCKED);
        public static string GCM_STATE_ACTIVE => nameof(GCM_STATE_ACTIVE);
        public static string GCM_STATE_PAUSED => nameof(GCM_STATE_PAUSED);
        public static string GCM_STATE_MAINTENANCE => nameof(GCM_STATE_MAINTENANCE);
        public static string GCM_STATE_STOPPED => nameof(GCM_STATE_STOPPED);
        public static string GCM_STATE_DISABLED => nameof(GCM_STATE_DISABLED);
        public static string GCM_STATE_GAME_COMPLETED => nameof(GCM_STATE_GAME_COMPLETED);
    }

    public static class DreamCubeScreenStates
    {
        public static string DC_NONE => nameof(DC_NONE);
        public static string DC_IDLE => nameof(DC_IDLE);
        public static string DC_WELCOME => nameof(DC_WELCOME);
        public static string DC_EXPERIENCE_INTRO => nameof(DC_EXPERIENCE_INTRO);
        public static string DC_PRACTICE_ROUND_INTRO => nameof(DC_PRACTICE_ROUND_INTRO);
        public static string DC_PRACTICE_ROUND_GAMEPLAY => nameof(DC_PRACTICE_ROUND_GAMEPLAY);
        public static string DC_ACTIVITY_SELECTION => nameof(DC_ACTIVITY_SELECTION);
        public static string DC_ACTIVITY_INTRO => nameof(DC_ACTIVITY_INTRO);
        public static string DC_ACTIVITY_GAMEPLAY => nameof(DC_ACTIVITY_GAMEPLAY);
        public static string DC_ACTIVITY_SCORE => nameof(DC_ACTIVITY_SCORE);
        public static string DC_EXPERIENCE_OUTRO => nameof(DC_EXPERIENCE_OUTRO);
        public static string DC_SESSION_SCORES => nameof(DC_SESSION_SCORES);
        public static string DC_MAINTENANCE => nameof(DC_MAINTENANCE);
        public static string DC_DISABLED => nameof(DC_DISABLED);
        public static string DC_STOPPED => nameof(DC_STOPPED);
        public static string DC_COMPLETED => nameof(DC_COMPLETED);
        public static string DC_ACTIVITY_GAMEPLAY_CTA => nameof(DC_ACTIVITY_GAMEPLAY_CTA);


        //public static string DC_ACTIVITY_FLASH_SCORE => nameof(DC_ACTIVITY_FLASH_SCORE);


        public static List<string> allScreenStates = new List<string>
        {
            DC_IDLE,
            DC_WELCOME,
            DC_EXPERIENCE_INTRO,
            //DC_PRACTICE_ROUND_INTRO,
            //DC_PRACTICE_ROUND_GAMEPLAY,
            DC_ACTIVITY_SELECTION,
            DC_ACTIVITY_INTRO,
            DC_ACTIVITY_GAMEPLAY,
            //DC_ACTIVITY_FLASH_SCORE,
            DC_ACTIVITY_SCORE,
            DC_EXPERIENCE_OUTRO,
            DC_SESSION_SCORES,
            DC_MAINTENANCE,
            DC_DISABLED,
            DC_STOPPED,
            DC_COMPLETED
        };


    }

  

    [Serializable]
    public class DreamCubeDataBase
    {
    }

    [Serializable]
    public class DreamCubeScreenStateData
    {
        public string DreamCubeID;
        public string CurrentDCState;
    }


    [Serializable]
    public class DreamCubeSessionData : DreamCubeDataBase
    {
        public string StartTime;
        public int Duration;
        public string DreamCubeID;
        public string SessionID;
        public string OwnerID;
        public string OwnerName;
        public string CurrentDCState;
        public string CurrentGCMState;
        public List<PlayerData> PlayerDataSet = new List<PlayerData>();
        public string ExtendQRCode;
        public bool ExtensionAllowed;
        public int RoundsPlayed;

        public override string ToString()
        {
            var obj = MemberwiseClone() as DreamCubeSessionData;
            return JsonUtility.ToJson(obj);
        }
    }


    [Serializable]
    public class GameRoundData : DreamCubeDataBase
    {
        public string DreamCubeID;
        public string SessionID;
        public LANGUAGE CurrentLanguage;
        public string CurrentCategoryID;
        public string CurrentActivityID;
        public string CurrentDifficulty;
        public PlayerData CurrentPlayerData;
        public int RoundsPlayed;

        public override string ToString()
        {
            var obj = MemberwiseClone() as GameRoundData;
            return JsonUtility.ToJson(obj);
        }
    }


    [Serializable]
    public class PlayerData : DreamCubeDataBase
    {
        public string PlayerName;
        public string PlayerID;
        public string PlayerTeam;
        public int PlayerQueue;
    }

    [Serializable]
    public class CurrentActivity : DreamCubeDataBase
    {
        public string CurrentCategoryID = ""; // the main category like dribbling
        public string CurrentActivityID = ""; // the option selected from that category
    }

    [Serializable]
    public class ScoreData : DreamCubeDataBase
    {
        public string SessionID;
        public string DreamCubeID;
        public string PlayerID;
        public string PlayerName;
        public int Score;

        public override string ToString()
        {
            var obj = MemberwiseClone() as ScoreData;
            return JsonUtility.ToJson(obj);
        }
    }
}





