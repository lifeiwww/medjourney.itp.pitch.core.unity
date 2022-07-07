using dreamcube.unity.Core.Scripts.API;
using UniRx;

namespace dreamcube.unity.Core.Scripts.Stores
{
    public static class DreamCubeSessionDataStore
    {
        public static readonly ReactiveProperty<DreamCubeSessionData> DreamCubeSessionData =
            new ReactiveProperty<DreamCubeSessionData>();

        public static readonly ReactiveProperty<string> CurrentGCMState =
            new ReactiveProperty<string>(GCMStates.GCM_STATE_NONE);

        public static readonly ReactiveProperty<string> CurrentDCScreenState =
            new ReactiveProperty<string>(DreamCubeScreenStates.DC_NONE);

        public static readonly ReactiveProperty<string> SessionId = new ReactiveProperty<string>();


        public static DreamCubeSessionData GetState()
        {
            return DreamCubeSessionData.Value;
        }

     

        public static void SetState(DreamCubeSessionData state)
        {
            if (state == null) return;

            DreamCubeSessionData.Value = state;
            CurrentDCScreenState.Value = state.CurrentDCState;
            CurrentGCMState.Value = state.CurrentGCMState;
            SessionId.Value = state.SessionID;
        }

        public static void SetCurrentDCScreenState(string state)
        {
            CurrentDCScreenState.Value = state;
            DreamCubeSessionData.Value.CurrentDCState = state;
        }
    }
}
