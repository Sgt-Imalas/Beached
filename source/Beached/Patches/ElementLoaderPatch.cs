﻿using Beached.Content;
using Beached.Utils;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Beached.Patches
{
    public class ElementLoaderPatch
    {
        [HarmonyPatch(typeof(ElementLoader), "Load")]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
            {
                // Add my new elements
                var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
                Elements.RegisterSubstances(list);
            }

            public static void Postfix()
            {
                // fix a bug with tags, IMPORTANT FOR NEW ELEMENTS
                ElementUtil.FixTags();

                // recolor water and salt water to be more watery and distinguishable
                var substanceTable = Assets.instance.substanceTable;
                substanceTable.GetSubstance(SimHashes.Water).colour = ModAssets.Colors.water;
                substanceTable.GetSubstance(SimHashes.SaltWater).colour = ModAssets.Colors.saltWater;
                
                Elements.AfterLoad();
            }

            [HarmonyPostfix]
            [HarmonyPriority(Priority.Last)]
            public static void LatePostfix()
            {
                var almostBlack = (Color)new Color32(0, 0, 1, 255);

                foreach (var substance in Assets.instance.substanceTable.list)
                {
                    if(substance.colour == Color.black)
                    {
                        substance.colour = almostBlack with { a = substance.colour.a };
                        Log.Debug("Set color of " + substance.name + " to ever so slightly lighter.");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        public class ElementLoader_CollectElementsFromYAML_Patch
        {
            public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                foreach (var elem in __result)
                {
                    // change Amber's melting target based on DLC
                    if (elem.elementId == Elements.amber.ToString() && DlcManager.FeatureRadiationEnabled())
                    {
                        elem.highTempTransitionTarget = SimHashes.Resin.ToString();
                    }
                    else if (elem.elementId == SimHashes.Diamond.ToString())
                    {
                        Mod.settings.CrossWorld.Elements.originalDiamondCategory = elem.materialCategory;
                    }
                    else if (elem.elementId == SimHashes.Katairite.ToString())
                    {
                        Mod.settings.CrossWorld.Elements.originalAbyssaliteCategory = elem.materialCategory;
                    }
                    else if (elem.elementId == SimHashes.Lime.ToString())
                    {
                        Mod.settings.CrossWorld.Elements.originalLimeHighTemp = elem.highTemp;
                        Mod.settings.CrossWorld.Elements.originalLimeHighTempTarget = elem.highTempTransitionTarget;

                    }
                    // reenable Helium
                    else if (elem.elementId == SimHashes.Helium.ToString())
                    {
                        elem.isDisabled = false;
                    }
                    // reenable Helium
                    else if (elem.elementId == SimHashes.LiquidHelium.ToString())
                    {
                        elem.isDisabled = false;
                    }
                }
            }
        }
    }
}