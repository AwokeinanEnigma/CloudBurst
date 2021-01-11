﻿using Cloudburst.Achievements;
using R2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudburst.Cores
{
    class AchievementCore
    {
        //is there nothing to feel
        //cause shit's getting weird
        //so to god who made this man
        //you better have one hell of a plan
        public static AchievementCore instance;

        public AchievementCore() => AddAchivements();

        protected void AddAchivements() {
            instance = this;
            LogCore.LogI("Initializing Core: " + base.ToString());

            //a normal human being wouldn't have to pretend to be normal
            //don'tcha think?
            AddTokens();

            UnlockablesAPI.AddUnlockable<GrabOrDie>(true);
            UnlockablesAPI.AddUnlockable<Paradox>(true);
            UnlockablesAPI.AddUnlockable<HitLevelCap>(false);
            UnlockablesAPI.AddUnlockable<WyattMasteryAchievement>(true);
        }
        protected void AddTokens() {
            LanguageAPI.Add("CLOUDBURST_GRABORDIE_ACHIEVEMENT_NAME", "Grab or Die!");
            LanguageAPI.Add("CLOUDBURST_GRABORDIE_UNLOCKABLE_NAME", "Grab or Die!");
            LanguageAPI.Add("CLOUDBURST_GRABORDIE_ACHIEVEMENT_DESC", "Grab an item under 30% health.");

            LanguageAPI.Add("CLOUDBURST_PARADOX_ACHIEVEMENT_NAME", "Paradox");
            LanguageAPI.Add("CLOUDBURST_PARADOX_UNLOCKABLE_NAME", "Paradox");
            LanguageAPI.Add("CLOUDBURST_PARADOX_ACHIEVEMENT_DESC", "As REX, repair the broken robot with an Escape Pod's Fuel Array.");

            LanguageAPI.Add("CLOUDBURST_HITLEVELCAP_ACHIEVEMENT_NAME", "One with the Planet");
            LanguageAPI.Add("CLOUDBURST_HITLEVELCAP_UNLOCKABLE_NAME", "One with the Planet");
            LanguageAPI.Add("CLOUDBURST_HITLEVELCAP_ACHIEVEMENT_DESC", "In a single run, reach level 35");

            LanguageAPI.Add("CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Custodian: Mastery");
            LanguageAPI.Add("CLOUDBURST_WYATT_MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Custodian: Mastery");
            LanguageAPI.Add("CLOUDBURST_WYATT_MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Custodian, beat the game or obliterate on Monsoon.");
        }

        public static string GetUnlockableString(string name) {
            if (CloudburstPlugin.EnableUnlockAll.Value) {
                //takes a village to fake a whole culture!
                return "";
            }
            var idToReturn = "";
            switch (name) {
                case "GrabOrDie":
                    idToReturn = "CLOUDBURST_GRABORDIE_REWARD_ID";
                    break;
                case "Paradox":
                    idToReturn = "CLOUDBURST_PARADOX_REWARD_ID";
                    break;
                case "HitLevelCap":
                    idToReturn = "CLOUDBURST_HITLEVELCAP_REWARD_ID";
                    break;
                case "WyattMastery":
                    idToReturn = "CLOUDBURST_WYATT_MONSOONUNLOCKABLE_REWARD_ID";
                    break;
            }
            return idToReturn;
        }
    }
}
