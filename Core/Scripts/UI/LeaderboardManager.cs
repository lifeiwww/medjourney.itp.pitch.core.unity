using DG.Tweening;
using dreamcube.unity.Core.Scripts.Stores;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dreamcube.unity.DefaultResources.Scripts.UI
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private GameObject holder;
        [SerializeField] private GameObject listPrefab;

        public void AddAllEntries(List<Tuple<string, int>> players, int currentScoreDiff = -1)
        {

            ClearEntries();

            if (!players.Any())
            {
                Log.Warning($"Received empty Leaderboard Data");
                return;
            }

            // sort list by score
            players = players.OrderByDescending(x => x.Item2).ToList();

            // find current player entry 
            var currentPlayerEntry =
                players.Where(x => x.Item1 == GameRoundDataStore.CurrentPlayerName.Value).ToList();

            Tuple<string, int> lastEntry = new Tuple<string, int>("",0);
            if ( currentPlayerEntry.Any())
            {
                lastEntry = currentPlayerEntry.Last();
            }


            int CurrentPlayerRating = -1;
            for (int i = 0; i < players.Count; i++)
            {
                bool isCurrent = false;
                if (players[i].Item1 == GameRoundDataStore.CurrentPlayerName.Value &&
                    players[i].Item2 == GameRoundDataStore.CurrentScore.Value)
                {
                    isCurrent = true;
                }
                if (isCurrent) CurrentPlayerRating = i;
            }

            int maxPlayersOnLeaderboard = players.Count > 10 ? 10 : players.Count;
            for (int i = 0; i < maxPlayersOnLeaderboard; i++)
            {
                bool isCurrent = i == CurrentPlayerRating;
                if (CurrentPlayerRating > 9 && i == 9 )
                {
                    AddPlayerEntry(CurrentPlayerRating, lastEntry, true);
                }
                else
                {
                    AddPlayerEntry(i, players[i], isCurrent);
                }
            }
        }

        public void AddPlayerEntry(int standing, Tuple<string, int> playerNameAndScore, bool isCurrent,
            int difference = -1)
        {
            GameObject entry;
            if (holder.transform.childCount > standing)
                entry = holder.transform.GetChild(standing).gameObject;
            else
                entry = Instantiate(listPrefab, holder.transform);

            entry.transform.SetParent(holder.transform);
            entry.transform.localScale = Vector3.one;
            entry.SetActive(true);

            // inject the information
            var info = entry.GetComponent<LeaderboardListObject>();
            info.setScore(playerNameAndScore.Item2);
            info.setName(playerNameAndScore.Item1);
            info.setStandings(standing + 1);
            if (difference > -1)
                info.setScoreDiff(difference);
            else
                info.setScoreDiff(-1);

            info.isCurrentPlayer(isCurrent);
        }

        public void ClearEntries()
        {
            //doesn't delete them, just hides them
            for (var i = 0; i < holder.transform.childCount; i++)
                holder.transform.GetChild(i).gameObject.SetActive(false);
        }

        public void AnimateIn()
        {
            //animate in
            var leaderboardSequence = DOTween.Sequence();
            leaderboardSequence.AppendInterval(0.5f);
            for (var i = 0; i < holder.transform.childCount; i++)
            {
                holder.transform.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
                leaderboardSequence.Append(holder.transform.GetChild(i).GetComponent<CanvasGroup>().DOFade(1, 0.25f));
            }
        }

        private void OnEnable()
        {
            AnimateIn();
        }
    }
}