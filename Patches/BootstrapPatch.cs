using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using HarmonyLib;
using Reptile;
using UnityEngine.SceneManagement;

namespace QuickLaunch.Patches
{
    [HarmonyPatch(typeof(Bootstrap))]
    internal class BootstrapPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Bootstrap.LaunchGame))]
        private static bool LaunchGame_Prefix(Bootstrap __instance)
        {
            __instance.StartCoroutine(SetupGameQuickLaunch(__instance));
            return false;
        }

        private static IEnumerator SetupGameQuickLaunch(Bootstrap bootstrap)
        {
            string coreSceneName = Path.GetFileNameWithoutExtension(bootstrap.sceneLibrary.coreScenePath);
            yield return bootstrap.LoadSceneAssets(coreSceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(coreSceneName));
            Core.Instance.StartCore(bootstrap.assets, bootstrap.gameVersion);
            Core.Instance.BaseModule.StartCoroutine(StartGameQuickLaunch(Core.Instance.BaseModule));
            yield break;
        }

        // This really damn ugly atm I think, should do some refactoring in the future but works well enough.
        private static IEnumerator StartGameQuickLaunch(BaseModule baseModule)
        {
            var mode = QuickLaunchPlugin.Instance.LaunchConfig.Mode;
            baseModule.ShowLoadingScreen();
            yield return baseModule.user.LoginASync();
            if (baseModule.user.IsLoggedIn)
            {
                yield return baseModule.user.LoadUserSaveData();
                var saveManager = baseModule.saveManager;
                baseModule.TriggerPostLoginAttemptEvents(baseModule.user.IsLoggedIn);
                baseModule.uiManager.InstantiateMainMenuSceneUI();

                var doNewGame = false;

                var slotForNewGame = saveManager.UsedSaveSlotCount;
                if (slotForNewGame >= 3)
                    slotForNewGame = -1;

                switch (mode)
                {
                    case QuickLaunchMode.LoadSaveInSlot1:
                        mode = QuickLaunchMode.LoadLastSave;
                        saveManager.SetCurrentSaveSlot(0);
                        break;

                    case QuickLaunchMode.LoadSaveInSlot2:
                        mode = QuickLaunchMode.LoadLastSave;
                        saveManager.SetCurrentSaveSlot(1);
                        break;

                    case QuickLaunchMode.LoadSaveInSlot3:
                        mode = QuickLaunchMode.LoadLastSave;
                        saveManager.SetCurrentSaveSlot(2);
                        break;
                }

                switch (mode)
                {
                    case QuickLaunchMode.LoadLastSave:
                        if (saveManager.HasCurrentSaveSlot)
                            baseModule.StartGameFromCurrentSaveSlotToStage(saveManager.CurrentSaveSlot.CurrentStage);
                        else
                        {
                            baseModule.uiManager.InstantiateMainMenuSceneUI();
                            ASceneSetupInstruction asceneSetupInstruction = baseModule.PrepareLoadMainMenuSceneInstruction();
                            baseModule.StartSceneSetup(asceneSetupInstruction);
                            while (baseModule.IsLoading)
                            {
                                yield return null;
                            }
                            yield break;
                        }
                        break;

                    case QuickLaunchMode.SkipIntro:
                        baseModule.uiManager.InstantiateMainMenuSceneUI();
                        ASceneSetupInstruction asceneSetupInstruction2 = baseModule.PrepareLoadMainMenuSceneInstruction();
                        baseModule.StartSceneSetup(asceneSetupInstruction2);
                        while (baseModule.IsLoading)
                        {
                            yield return null;
                        }
                        yield break;

                    case QuickLaunchMode.CreateNewSaveInSlot1:
                        doNewGame = true;
                        slotForNewGame = 0;
                        break;
                    case QuickLaunchMode.CreateNewSaveInSlot2:
                        doNewGame = true;
                        slotForNewGame = 1;
                        break;
                    case QuickLaunchMode.CreateNewSaveInSlot3:
                        doNewGame = true;
                        slotForNewGame = 2;
                        break;
                    case QuickLaunchMode.CreateNewSaveInFreeSlot:
                        doNewGame = true;
                        break;
                }

                if (doNewGame)
                {
                    if (slotForNewGame < 0 || slotForNewGame >= 3)
                    {
                        baseModule.uiManager.InstantiateMainMenuSceneUI();
                        ASceneSetupInstruction asceneSetupInstruction2 = baseModule.PrepareLoadMainMenuSceneInstruction();
                        baseModule.StartSceneSetup(asceneSetupInstruction2);
                        while (baseModule.IsLoading)
                        {
                            yield return null;
                        }
                        yield break;
                    }
                    else
                    {
                        baseModule.StartNewGameForSaveSlot(slotForNewGame);
                    }
                }
                
            }
            else
            {
                // fallback, couldn't even log in.
                baseModule.GoBackToIntro();
            }
            yield break;
        }
    }
}
