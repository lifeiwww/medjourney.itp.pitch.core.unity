using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.Signals.Events;
using Serilog;
using UnityEngine;

namespace dreamcube.unity.Core.Scripts.Signals.SignalRClient.Client
{
    public class SignalREventSimulatorBase : MonoBehaviour
    {
        protected GameBayData fakeGameData;
        protected TeamStructureData fakeTeamStructureData;

          protected virtual void TestAction(ACTIONS_TYPE theAction)
        {
            var data = new IntData {AInt = (int) theAction};
            EventManager.Instance.TriggerEvent(EventStrings.EventOnSignalRMessage, SignalRIn.NotifyOnGameActionChange, null,
                data);
        }

        protected virtual void OnTaskComplete(string msg = "")
        {
            Log.Debug($"OnTaskComplete {msg}");
        }

        protected virtual void PopulateFakeData()
        {
            // create player 
            var players = new List<PlayerScoreData>();
            players.Add(new PlayerScoreData
            {
                PlayerID = 1,
                PlayerName = "Test Name",
                PlayerScore = 100,
                PlayerQueue = 1,
                TeamName = "Red Devils"
            });

            var teamData = new TeamData
            {
                PlayersScoreData = players
            };

            var Teams = new List<TeamData>();
            Teams.Add(teamData);

            fakeTeamStructureData = new TeamStructureData
            {
                Teams = Teams
            };

            fakeGameData = new GameBayData
            {
                GameEventID = 1,
                CurrentGameState = GAME_BAY_STATES.GAME_BAY_STATE_ACTIVE,
                CurrentTeam = "Red Devils",
                CurrentPlayerID = 1,
                CurrentLevel = 1,
                CurrentStory = 1,
                TeamStructureData = fakeTeamStructureData
            };
        }
    }
}