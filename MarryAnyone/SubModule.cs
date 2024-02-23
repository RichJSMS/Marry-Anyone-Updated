﻿using HarmonyLib;
using MarryAnyone.Behaviors;
using MarryAnyone.Models;
using MarryAnyone.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MarryAnyone
{
    internal sealed class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("mod.bannerlord.anyone.marry-updated").PatchAll();
            MAConfig.Initialize();

            InformationManager.DisplayMessage(new InformationMessage($"Marry Anyone loaded", Colors.Green));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter)gameStarterObject;
                campaignGameStarter.AddBehavior(new MARomanceCampaignBehavior());
                campaignGameStarter.AddModel(new MAMarriageModel());
            }
        }
    }
}