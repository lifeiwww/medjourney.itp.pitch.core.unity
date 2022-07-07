using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.API;
using UniRx;

namespace dreamcube.unity.Core.Scripts.Stores
{
    public static class GameRoundDataStore
    {
        public static readonly ReactiveProperty<GameRoundData> CurrentGameRoundData = new ReactiveProperty<GameRoundData>();
        public static readonly ReactiveProperty<string> SessionId = new ReactiveProperty<string>();
        public static readonly ReactiveProperty<LANGUAGE> CurrentLanguage = new ReactiveProperty<LANGUAGE>(LANGUAGE.LANGUAGE_ZH);

        public static readonly ReactiveProperty<string> CurrentTeamName = new ReactiveProperty<string>();
        public static readonly ReactiveProperty<string> CurrentPlayerName = new ReactiveProperty<string>();
        public static readonly ReactiveProperty<string> CurrentCategory = new ReactiveProperty<string>();
        public static readonly ReactiveProperty<string> CurrentActivityID = new ReactiveProperty<string>();
        public static readonly ReactiveProperty<int> CurrentActivityNumber = new ReactiveProperty<int>();
        public static readonly ReactiveProperty<string> CurrentDifficulty = new ReactiveProperty<string>();

        public static readonly ReactiveProperty<int> CurrentScore = new ReactiveProperty<int>(0);
        public static readonly ReactiveProperty<List<ScoreData>> AllScores = new ReactiveProperty<List<ScoreData>>();

        public static GameRoundData GetRoundData()
        {
            return CurrentGameRoundData.Value;
        }

        public static void SetScore(int value)
        {
            CurrentScore.SetValueAndForceNotify(value);
        }

        public static void SetAllScore(List<ScoreData> scores)
        {
            AllScores.Value = scores;
        }

        public static void SetRoundData(GameRoundData roundData)
        {
            if (roundData == null)
                return;

            CurrentGameRoundData.Value = roundData;
            SessionId.Value = roundData.SessionID;
            CurrentLanguage.Value = roundData.CurrentLanguage;
            CurrentDifficulty.Value = roundData.CurrentDifficulty;
            CurrentCategory.Value = roundData.CurrentCategoryID;
            CurrentActivityID.Value = roundData.CurrentActivityID;
            CurrentPlayerName.Value = roundData.CurrentPlayerData.PlayerName;
        }

        public static void SetCurrentLanguage(LANGUAGE language)
        {
            CurrentLanguage.Value = language;
        }

    }
}
