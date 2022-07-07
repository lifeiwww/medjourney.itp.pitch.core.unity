using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;



using dreamcube.unity.Core.Scripts;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Activity;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Category;
using dreamcube.unity.Core.Scripts.API.ContentAPI.Screen;
using dreamcube.unity.Core.Scripts.API.SystemTextJsonSamples;
using dreamcube.unity.Core.Scripts.Configuration.GeneralConfig;
using dreamcube.unity.Core.Scripts.Models.ContentModels;
using dreamcube.unity.Core.Scripts.Stores;
using Serilog;

using Random = UnityEngine.Random;

public enum testEnum
{
    Idle_01 = 0,
    Welcome_01,
    ExperienceIntro_01,
    PracticeRoundIntro_01
}


namespace dreamcube.unity.Core.Scripts.API
{
    public static class GenerateFakeAPIData
    {
        public static GameRoundData GenerateFakeGameRoundData()
        {
            var faker = new Faker();
            var playerList = GenerateFakePlayerSetData(Random.Range(1, 12));
            PlayerData currentPlayer = playerList[Random.Range(0, playerList.Count)];

            GameRoundData roundData = new GameRoundData()
            {
                DreamCubeID = ConfigManager.Instance.generalSettings.DreamCube, //faker.IndexFaker,
                SessionID = faker.Random.AlphaNumeric(10),
                CurrentDifficulty = "Easy",
                CurrentLanguage = faker.Random.Enum<LANGUAGE>(), // LANGUAGE.LANGUAGE_ZH, faker.Ran
                CurrentCategoryID = "Passing", 
                CurrentActivityID = $"Passing 0{Random.Range(1,5)}",
                RoundsPlayed = faker.Random.Number(0, 5),
                CurrentPlayerData = currentPlayer
            };

            Log.Debug($"GenerateFakeGameRoundData: -- {roundData}");
            return roundData;
        }

        public static DreamCubeScreenStateData GenerateFakeDreamCubeScreenStateData()
        {
            var faker = new Faker();

            DreamCubeScreenStateData state = new DreamCubeScreenStateData
            {
                CurrentDCState = faker.Lorem.Sentence(),
                DreamCubeID = ConfigManager.Instance.generalSettings.DreamCube, //faker.IndexFaker,
            };
            return state;
        }


        public static DreamCubeSessionData GenerateFakeDreamCubeSessionData( bool setIdol = true)
        {
            Log.Debug($"{nameof(GenerateFakeDreamCubeSessionData)}");
            var faker = new Faker();


            //var randomScreen = DreamCubeScreenStates.allScreenStates[Random.Range(0, DreamCubeScreenStates.allScreenStates.Count)];


            var dcState = setIdol ? DreamCubeScreenStates.DC_IDLE : DreamCubeSessionDataStore.CurrentDCScreenState.Value;

            DreamCubeSessionData dreamCubeData = new DreamCubeSessionData
            {
                StartTime = faker.Date.Soon().ToString(CultureInfo.CurrentCulture), //  DateTime.UtcNow,
                CurrentGCMState = dcState,
                CurrentDCState = DreamCubeScreenStates.DC_IDLE, //randomScreen,
                DreamCubeID = ConfigManager.Instance.generalSettings.DreamCube, //faker.IndexFaker,
                SessionID = faker.Random.AlphaNumeric(10),
                Duration = faker.Random.Number(20, 120),
                ExtendQRCode = faker.Internet.Url(),
                ExtensionAllowed = faker.Random.Bool(),
                OwnerID = faker.Random.AlphaNumeric(10),
                RoundsPlayed = faker.Random.Number(0, 5),
                PlayerDataSet = new List<PlayerData>(),
                OwnerName = faker.Name.FirstName()

            };

            for (int i = 0; i < Random.Range(1, 13); i++)
            {
                dreamCubeData.PlayerDataSet.Add(GenerateFakePlayerData());
            }

            dreamCubeData.OwnerName = dreamCubeData.PlayerDataSet.FirstOrDefault().PlayerName;
            Log.Debug($"GenerateFakeDreamCubeSessionData: {dreamCubeData}");
            return dreamCubeData;
        }

        public static PlayerData GenerateFakePlayerData()
        {
            var faker = new Faker();

            PlayerData playerData = new PlayerData()
            {
                PlayerID = faker.Random.AlphaNumeric(10),
                PlayerName = faker.Name.FirstName(),
                PlayerTeam = faker.Company.CompanyName(),
                PlayerQueue = 1
            };
            return playerData;
        }

        public static List<PlayerData> GenerateFakePlayerSetData(int numPlayers)
        {

            List<PlayerData> playerDataSet = new List<PlayerData>();
            for (int i = 0; i < numPlayers; i++)
            {
                var player = GenerateFakePlayerData();
                playerDataSet.Add(player);
            }

            return playerDataSet;
        }

        public static List<Tuple<string, int>> GenerateFakeLeaderboardData()
        {

            var faker = new Faker();
            var fakeLeaderBoardData = new List<Tuple<string, int>>();

            List<PlayerData> playerDataList =
                DreamCubeSessionDataStore.DreamCubeSessionData.Value.PlayerDataSet.ToList();
            foreach (var playerData in playerDataList)
            {

                var score = playerData == GameRoundDataStore.GetRoundData().CurrentPlayerData
                    ? GameRoundDataStore.CurrentScore.Value
                    : faker.Random.Number(0, 2000);

                if (playerData == GameRoundDataStore.GetRoundData().CurrentPlayerData)
                {
                    var pair = Tuple.Create(playerData.PlayerName, score);
                    fakeLeaderBoardData.Add(pair);
                }

            }

            return fakeLeaderBoardData;
        }

        public static SharedSchema.Title GenerateFakeTitle()
        {
            var englishFaker = new Faker();
            var chineseFaker = new Faker("zh_CN");
            SharedSchema.Title title = new SharedSchema.Title
            {
                en = englishFaker.Lorem.Sentence(3),
                zh = chineseFaker.Lorem.Sentence(2)
            };
            return title;
        }

        public static Activity GenerateFakeActivity()
        {
            var faker = new Faker();
            //Activity fakeActivity = faker.Generate();
            Activity fakeActivity = new Activity
            {
                ID = $"Passing 0{Random.Range(1, 5)}",
                Text = new SharedSchema.Text() {Title = GenerateFakeTitle()},
                Media = new ContentAPI.Activity.Media(),
                PrefabFilename = faker.Hacker.Phrase() + ".prefab"
            };
            Log.Debug($"generated fake activity {fakeActivity}");
            return fakeActivity;
        }

        public class FakeDreamCubeSessionData : DreamCubeDataBase
        {
            public string OwnerID;
            public testEnum CurrentDCState;
            public string CurrentGCMState;
        }

        public static ScoreData GenerateFakePlayerScoreData()
        {
            var faker = new Faker();

            var currentPlayers = DreamCubeSessionDataStore.GetState().PlayerDataSet;
            var randomPlayerNumber = Random.Range(0, currentPlayers.Count);
            var currentPlayer = currentPlayers[randomPlayerNumber];

            ScoreData scoreData = new ScoreData
            {
                SessionID = DreamCubeSessionDataStore.GetState().SessionID,
                DreamCubeID = ConfigManager.Instance.generalSettings.DreamCube,
                PlayerID = currentPlayer.PlayerID,
                PlayerName = currentPlayer.PlayerName,
                Score = Random.Range(456, 2105)
            };
            Log.Debug($"GenerateFakePlayerScoreData {scoreData}");

            return scoreData;
        }

        public static string GenerateEnumTest()
        {
            FakeDreamCubeSessionData data = new FakeDreamCubeSessionData()
            {
                OwnerID = "fdassdfgsdf",
                CurrentDCState = testEnum.PracticeRoundIntro_01,
                CurrentGCMState = "sdffsdfsdsfd"
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var jsonString = JsonSerializer.Serialize(data, options);
            return jsonString;
        }

    }


    namespace SystemTextJsonSamples
    {
        public class UpperCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                name.ToUpper();
        }
    }


}
