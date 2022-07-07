using System;
using System.Collections.Generic;
using System.Linq;
using dreamcube.unity.Core.Scripts.Signals.Events;
using dreamcube.unity.Core.Scripts.Stores;
using OdinSerializer.Utilities;
using Serilog;
using UniRx;
using UnityEngine;
using static dreamcube.unity.Core.Scripts.API.DreamCubeScreenStates;

namespace dreamcube.unity.Core.Scripts.UI
{

    [Serializable]
    public class LinearMediaPlayerLookup
    {
        public ScreenTypeEnum screenType;
        public VideoController videoController;
        public bool ShouldTransition = true;
    }

    public class ScreenNameLookup
    {
        public static ScreenTypeEnum GetScreenTypeEnum(string screenStateName)
        {
            switch (screenStateName)
            {
                case nameof(DC_IDLE):
                    return ScreenTypeEnum.Idle_01;
                case nameof(DC_WELCOME):
                    return ScreenTypeEnum.Welcome_01;
                case nameof(DC_EXPERIENCE_INTRO):
                    return ScreenTypeEnum.ExperienceIntro_01;
                case nameof(DC_PRACTICE_ROUND_INTRO):
                    return ScreenTypeEnum.PracticeRoundIntro_01;
                case nameof(DC_PRACTICE_ROUND_GAMEPLAY):
                    return ScreenTypeEnum.PracticeRoundGameplay_01;
                case nameof(DC_ACTIVITY_SELECTION):
                    return ScreenTypeEnum.DrillSelection_01;
                case nameof(DC_ACTIVITY_INTRO):
                    return ScreenTypeEnum.DrillIntroduction_01;
                case nameof(DC_ACTIVITY_GAMEPLAY):
                    return ScreenTypeEnum.Exercise_01;
                case nameof(DC_ACTIVITY_SCORE):
                    return ScreenTypeEnum.Leaderboard_01;
                case nameof(DC_EXPERIENCE_OUTRO):
                    return ScreenTypeEnum.ExperienceOutro_01;
                case nameof(DC_SESSION_SCORES):
                    return ScreenTypeEnum.EndofGame_01;
                case nameof(DC_MAINTENANCE):
                    return ScreenTypeEnum.Maintenance;
                case nameof(DC_DISABLED):
                    return ScreenTypeEnum.Offline;
                case nameof(DC_ACTIVITY_GAMEPLAY_CTA):
                    return ScreenTypeEnum.ActivityCTA;
                default:
                    return ScreenTypeEnum.None;
            }
        }


        public static string GetScreenTypeName(ScreenTypeEnum screenState)
        {
            switch (screenState)
            {
                case ScreenTypeEnum.Idle_01:
                    return nameof(DC_IDLE);
                case ScreenTypeEnum.Welcome_01:
                    return nameof(DC_WELCOME);
                case ScreenTypeEnum.ExperienceIntro_01:
                    return nameof(DC_EXPERIENCE_INTRO);
                case ScreenTypeEnum.PracticeRoundIntro_01:
                    return nameof(DC_PRACTICE_ROUND_INTRO);
                case ScreenTypeEnum.PracticeRoundGameplay_01:
                    return nameof(DC_PRACTICE_ROUND_GAMEPLAY);
                case ScreenTypeEnum.DrillSelection_01:
                    return nameof(DC_ACTIVITY_SELECTION);
                case ScreenTypeEnum.DrillIntroduction_01:
                    return nameof(DC_ACTIVITY_INTRO);
                case ScreenTypeEnum.Exercise_01:
                    return nameof(DC_ACTIVITY_GAMEPLAY);
                case ScreenTypeEnum.Leaderboard_01:
                    return nameof(DC_ACTIVITY_SCORE);
                case ScreenTypeEnum.ExperienceOutro_01:
                    return nameof(DC_EXPERIENCE_OUTRO);
                case ScreenTypeEnum.EndofGame_01:
                    return nameof(DC_SESSION_SCORES);
                case ScreenTypeEnum.Maintenance:
                    return nameof(DC_MAINTENANCE);
                case ScreenTypeEnum.Offline:
                    return nameof(DC_DISABLED);
                case ScreenTypeEnum.ActivityCTA:
                    return nameof(DC_ACTIVITY_GAMEPLAY_CTA);
                default:
                    return string.Empty;
            }
        }

    }

    public abstract class UIScreenFlowBase : MonoBehaviour
    {
        [SerializeField] protected List<UIScreenIdentifier> screens = new List<UIScreenIdentifier>();
        [SerializeField] protected List<LinearMediaPlayerLookup> linearMediaPlayerLookup = new List<LinearMediaPlayerLookup>();

        protected ScreenTypeEnum CurrentScreen = ScreenTypeEnum.None;
        protected ScreenTypeEnum PreviousScreen = ScreenTypeEnum.None;

        protected string _currentDreamCubeScreenState = DC_IDLE;

        //protected abstract ScreenTypeEnum GetScreenTypeEnum(string screenState);
        protected abstract string GetNextScreenStateName(ScreenTypeEnum screenEnum);

        protected virtual void Setup()
        {
            if (!screens.Any())
                screens = GetComponentsInChildren<UIScreenIdentifier>(includeInactive: true).ToList();

            DreamCubeSessionDataStore.CurrentDCScreenState.Subscribe(x =>
            {
                var screenState = DreamCubeSessionDataStore.CurrentDCScreenState.Value;
                SwitchScreen(screenState);
            });
        }

        protected virtual void UpdateScreenState(ScreenTypeEnum screenEnum)
        {
            var nextScreenStateState = GetNextScreenStateName(screenEnum);
            UpdateScreenStateStore(nextScreenStateState);
        }

        protected virtual void UpdateScreenStateStore(string nextScreenStateState)
        {
            // make sure the next screen exists
            var nextName = ScreenNameLookup.GetScreenTypeEnum(nextScreenStateState);
            var screen = screens.Find(i => i.screenTypeEnum == nextName);
            if (!screen.SafeIsUnityNull())
            {
                DreamCubeSessionDataStore.SetCurrentDCScreenState(nextScreenStateState);
                EventManager.Instance.TriggerEvent(EventStrings.EventOnPitchTriggeredScreenStateChange);
            }
        }

        protected virtual bool SetNextScreen(string screenState, out ScreenTypeEnum nextScreen)
        {
            nextScreen = ScreenNameLookup.GetScreenTypeEnum(screenState);

            if (nextScreen == ScreenTypeEnum.None)
                return false;

            if (_currentDreamCubeScreenState == screenState)
                return false;

            if (CurrentScreen == nextScreen)
                return false;

            Log.Debug($"{nameof(SetNextScreen)} next-> {nextScreen} | screenState-> {screenState}");

            PreviousScreen = CurrentScreen;
            CurrentScreen = nextScreen;
            _currentDreamCubeScreenState = screenState;
            return true;
        }


        protected virtual void SetLinearMediaForScreen(ScreenTypeEnum screenEnum)
        {
            var foundItems = linearMediaPlayerLookup.Where(x => x.screenType == screenEnum);
            foreach (var item in foundItems)
            {
                var controller = item.videoController;
                var screenID = ScreenNameLookup.GetScreenTypeName(screenEnum);
                var screenVideoUrlWithId = ScreenDataStore.GetScreenVideoURLWithID(screenID);

                var vidUrl = GameRoundDataStore.CurrentLanguage.Value == API.LANGUAGE.LANGUAGE_EN ? screenVideoUrlWithId.en: screenVideoUrlWithId.zh;
                Log.Debug($"{nameof(SetLinearMediaForScreen)} screenEnum {screenEnum} | url {vidUrl}");

                controller.SetPlayerURL(vidUrl);
            }
        }




        // override this for fancy animations
        protected virtual void SwitchScreen(string screenState)
        {
            if (!SetNextScreen(screenState, out var nextScreen)) return;

            DoSwitch(nextScreen);
        }

        protected virtual void DoSwitch(ScreenTypeEnum nextScreen)
        {
            foreach (var screen in screens)
            {
                var setActive = screen.screenTypeEnum == nextScreen;
                screen.gameObject.SetActive(setActive);
            }
        }
    }
}
