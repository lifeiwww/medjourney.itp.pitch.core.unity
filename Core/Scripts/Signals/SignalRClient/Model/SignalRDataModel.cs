using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MessagePack;



public enum LANGUAGE
{
    LANGUAGE_ZH = 0,
    LANGUAGE_EN
}

public enum GAME_BAY_STATES
{
    GAME_BAY_STATE_NONE = 0,
    GAME_BAY_STATE_IDLE,
    GAME_BAY_STATE_ACTIVE,
    GAME_BAY_STATE_PAUSED,
    GAME_BAY_STATE_MAINTENANCE,
    GAME_BAY_STATE_STOPPED,
    GAME_BAY_STATE_DISABLED,
    GAME_BAY_STATE_ASSIGNED,
    GAME_BAY_STATE_ENDED,
    GAME_BAY_STATE_UNLOCKED,
    GAME_BAY_STATE_STORY_ENDED
}


public enum PLAYER_MODE
{
    PLAYER_MODE_NONE = 0,
    PLAYER_MODE_INDIVIDUAL,
    PLAYER_MODE_TEAM
}

public enum BONUS_TYPE
{
    BONUS_TYPE_NONE = 0,
    BONUS_TYPE_BALL,
    BONUS_TYPE_SHOE,
    BONUS_TYPE_STAR
}

public enum STORY_TYPE
{
    STORY_TYPE_A = 1,
    STORY_TYPE_B,
    STORY_TYPE_C
}

//public enum EMOJI_TYPE
//{
//    EMOJI_TYPE_HEARTS = 0,
//    EMOJI_TYPE_SAD,
//    EMOJI_TYPE_DEVIL,
//    EMOJI_TYPE_FINGER,
//    EMOJI_TYPE_HORN
//}

public enum ACTIONS_TYPE
{
    ACTIONS_TYPE_REFRESH = 0,
    ACTIONS_TYPE_IDLE,
    ACTIONS_TYPE_PLAY,
    ACTIONS_TYPE_PAUSE,
    ACTIONS_TYPE_MAINTENANCE,
    ACTIONS_TYPE_STOP,
    ACTIONS_TYPE_DISABLED,
    ACTIONS_TYPE_SKIP_PLAYER,
    ACTIONS_TYPE_ENABLE_EXTEND,
    ACTIONS_TYPE_DISABLE_EXTEND
}

[MessagePackObject]
public class DataModelBase
{
}

[MessagePackObject]
public class IntData : DataModelBase
{
    [Key("aInt")] public int AInt { get; set; }

    public override string ToString()
    {
        return $"AInt: {AInt}";
    }
}

[MessagePackObject]
public class FloatData : DataModelBase
{
    [Key("aFloat")] public float AFloat { get; set; }

    public override string ToString()
    {
        return $"AFloat: {AFloat}";
    }
}

[MessagePackObject]
public class BoolData : DataModelBase
{
    [Key("aBool")] public bool ABool { get; set; } = false;

    public override string ToString()
    {
        return $"ABool: {ABool}";
    }
}

[MessagePackObject]
public class ListPlayerScores : DataModelBase
{
    [Key("list")] public List<PlayerScoreData> List { get; set; }

    public override string ToString()
    {
        return $"List: {string.Join(", ", List)}";
    }
}

//[MessagePackObject]
//public class PlayerData : DataModelBase
//{

//    public override string ToString()
//    {
//        return $"PlayerName: {PlayerName}, PlayerID: {PlayerID}";
//    }
//}

[MessagePackObject]
public class PlayerScoreData : DataModelBase
{
    [Key("teamName")] [CanBeNull] public string TeamName { get; set; } = "";
    [Key("gameEventID")] public int GameEventID { get; set; }
    [Key("playerQueue")] public int PlayerQueue { get; set; }
    [Key("playerName")] [CanBeNull] public string PlayerName { get; set; } = "";
    [Key("playerID")] public int PlayerID { get; set; }
    [Key("playerScore")] public int PlayerScore { get; set; }
    [Key("levelId")] public int LevelID { get; set; }
    [Key("storyCounter")] public int StoryCounter { get; set; }
    [Key("targetsHit")] public int TargetsHit { get; set; }

    [Key("bonusesCollected")]
    [CanBeNull]
    public List<BonusItemData> BonusesCollected { get; set; } = new List<BonusItemData> {new BonusItemData()};

    public override string ToString()
    {
        return
            $"TeamName: {TeamName}, GameEventID:{GameEventID} ,PlayerName:{PlayerName},PlayerQueue:{PlayerQueue}, PlayerID: {PlayerID}, LevelID:{LevelID}, StoryCounter:{StoryCounter} PlayerScore:{PlayerScore},TargetsHit: {TargetsHit}, BonusesCollected:{string.Join(", ", BonusesCollected)}";
    }
}

[MessagePackObject]
public class TeamData : DataModelBase
{
    [Key("playersScoreData")] public List<PlayerScoreData> PlayersScoreData { get; set; }

    public override string ToString()
    {
        return $"PlayersScoreData: {string.Join(", ", PlayersScoreData)}";
    }
}

[MessagePackObject]
public class TeamStructureData : DataModelBase
{
    [Key("gameBayID")] public int GameBayID { get; set; }
    [Key("teams")] public List<TeamData> Teams { get; set; } = new List<TeamData>();

    public override string ToString()
    {
        return $"GameBayID: {GameBayID}, Teams: {string.Join(", ", Teams)}";
    }
}


[MessagePackObject]
public class GameBayData : DataModelBase
{
    [Key("startTime")] [CanBeNull] public DateTime? StartTime { get; set; } = null;
    [Key("duration")] public int Duration { get; set; }
    [Key("gameBayID")] public int GameBayID { get; set; }
    [Key("gameEventID")] public int GameEventID { get; set; }
    [Key("currentGameState")] public GAME_BAY_STATES CurrentGameState { get; set; }
    [Key("currentStory")] public int CurrentStory { get; set; }
    [Key("currentLevel")] public int CurrentLevel { get; set; }
    [Key("ownerId")] public int OwnerId { get; set; }

    [Key("ownerName")] public string OwnerName { get; set; }
    [Key("currentTeam")] public string CurrentTeam { get; set; } = "";
    [Key("currentPlayerID")] public int CurrentPlayerID { get; set; }

    [Key("teamStructureData")]
    [CanBeNull]
    public TeamStructureData TeamStructureData { get; set; } = new TeamStructureData();

    [Key("resetMode")] public bool ResetMode { get; set; } = false;
    [Key("extendQRCode")] public string ExtendQRCode { get; set; } = "";
    [Key("extensionAllowed")] public bool ExtensionAllowed { get; set; } = true;
    [Key("offlineMode")] public bool OfflineMode { get; set; } = false;
    [Key("testMode")] public bool TestMode { get; set; }
    [Key("storyCounter")] public int StoryCounter { get; set; }


    public override string ToString()
    {
        return
            $"StartTime:{StartTime},Duration:{Duration}, GameBayID: {GameBayID}, GameEventID: {GameEventID},OwnerId: {OwnerId}, OwnerName:{OwnerName}, CurrentGameState: {CurrentGameState} CurrentStory: {CurrentStory}, CurrentLevel: {CurrentLevel}, CurrentPlayerID: {CurrentPlayerID}, TeamStructureData: {TeamStructureData}, resetMode: {ResetMode}, extendQRCode: {ExtendQRCode}, ExtensionAllowed: {ExtensionAllowed}, OfflineMode: {OfflineMode}, TestMode:{TestMode}, StoryCounter: {StoryCounter}";
    }
}

[MessagePackObject]
public class ExtendGameData : DataModelBase
{
    [Key("gameBayID")] public int GameBayID { get; set; }
    [Key("extendAmount")] public int ExtendAmount { get; set; }

    public override string ToString()
    {
        return $"GameBayID: {GameBayID}, ExtendAmount: {ExtendAmount}";
    }
}

[MessagePackObject]
public class BonusItemData : DataModelBase
{
    [Key("bonusEnum")] public BONUS_TYPE BonusEnum { get; set; }
    [Key("bonusScore")] public int BonusScore { get; set; }
    [Key("levelId")] public int LevelId { get; set; }


    public override string ToString()
    {
        return $"BonusEnum: {BonusEnum}, BonusScore: {BonusScore}, LevelId:{LevelId}";
    }
}

[MessagePackObject]
public class CameraData : DataModelBase
{
    [Key("cameraID")] public string CameraID { get; set; }
    [Key("cameraSerialNumber")] public string CameraSerialNumber { get; set; }
    [Key("cameraAlignmentStatus")] public bool CameraAlignmentStatus { get; set; }

    public override string ToString()
    {
        return
            $"CameraID: {CameraID}, CameraSerialNumber:{CameraSerialNumber}, CameraAlignmentStatus:{CameraAlignmentStatus}";
    }
}

[MessagePackObject]
public class TrackingSystemData : DataModelBase
{
    [Key("gameBayID")] public int GameBayID { get; set; }
    [Key("cameras")] public List<CameraData> Cameras { get; set; }


    public override string ToString()
    {
        return $"GameBayID: {GameBayID}, Cameras: {string.Join(", ", Cameras)}";
    }
}