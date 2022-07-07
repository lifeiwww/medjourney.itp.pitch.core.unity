using System.Collections.Generic;
using dreamcube.unity.Core.Scripts.API;

namespace dreamcube.unity.Core.Scripts.Stores
{
    public static class StoreUtil
    {

        public static bool IsDebug;

        public static List<string> GetPlayersNames()
        {
            var dreamCubeSessionData = DreamCubeSessionDataStore.GetState();
            var names = new List<string>();
            if (dreamCubeSessionData == null) return names;

            foreach (var player in dreamCubeSessionData.PlayerDataSet)
            {
                names.Add(player.PlayerName);
            }
            return names;
        }
    }
}
