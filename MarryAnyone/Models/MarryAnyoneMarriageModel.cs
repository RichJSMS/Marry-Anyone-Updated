﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using System.Linq;
using MarryAnyone.Patches;

namespace MarryAnyone.Models
{
    internal class MarryAnyoneMarriageModel : DefaultMarriageModel
    {
        private static DefaultMarriageModel? _instance;

        public override bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero)
        {
            if (_instance is null)
            {
                _instance = new();
            }

            // Does not directly call IsSuitableForMarriage, instead calls CanMarry
            bool canMarry = firstHero.CanMarry() && secondHero.CanMarry();
            if (!canMarry)
            {
                return false;
            }

            bool isMainHero = firstHero == Hero.MainHero || secondHero == Hero.MainHero;

            // Section for AI heroes from the original method
            if (!isMainHero)
            {
                base.IsCoupleSuitableForMarriage(firstHero, secondHero);
            }

            // Section for Marry Anyone method
            MASettings settings = new();
            bool isHeterosexual = settings.SexualOrientation == "Heterosexual";
            bool isHomosexual = settings.SexualOrientation == "Homosexual";
            bool isIncestuous = settings.Incest;
            bool discoverAncestors = DefaultMarriageModelPatches.DiscoverAncestors(_instance, firstHero, 3).Intersect(DefaultMarriageModelPatches.DiscoverAncestors(_instance, secondHero, 3)).Any();

            if (!isIncestuous)
            {
                if (discoverAncestors)
                {
                    return false;
                }
            }

            bool isAttracted = true;
            if (isHeterosexual)
            {
                isAttracted = firstHero.IsFemale != secondHero.IsFemale;
            }
            if (isHomosexual)
            {
                isAttracted = firstHero.IsFemale == secondHero.IsFemale;
            }
            return isAttracted;
        }

        public override bool IsSuitableForMarriage(Hero maidenOrSuitor)
        {
            MASettings settings = new();
            bool isCheating = false;
            bool isPolygamous = false;

            if (Hero.OneToOneConversationHero is not null)
            {
                bool inConversation = maidenOrSuitor == Hero.MainHero || maidenOrSuitor == Hero.OneToOneConversationHero;
                if (inConversation)
                {
                    isCheating = settings.Cheating;
                    isPolygamous = settings.Polygamy;
                }
            }
            if (!maidenOrSuitor.IsAlive || maidenOrSuitor.IsNotable || maidenOrSuitor.IsTemplate)
            {
                return false;
            }
            if ((maidenOrSuitor.Spouse is null && !maidenOrSuitor.ExSpouses.Any(exSpouse => exSpouse.IsAlive)) || isPolygamous || isCheating)
            {
                if (maidenOrSuitor.IsFemale)
                {
                    return maidenOrSuitor.CharacterObject.Age >= Campaign.Current.Models.MarriageModel.MinimumMarriageAgeFemale;
                }
                return maidenOrSuitor.CharacterObject.Age >= Campaign.Current.Models.MarriageModel.MinimumMarriageAgeMale;
            }
            return false;
        }
    }
}