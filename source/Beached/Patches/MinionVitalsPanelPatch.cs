﻿using Beached.Content.ModDb;
using HarmonyLib;

namespace Beached.Patches
{
	public class MinionVitalsPanelPatch
	{
		[HarmonyPatch(typeof(MinionVitalsPanel), nameof(MinionVitalsPanel.Init))]
		public class MinionVitalsPanel_Init_Patch
		{
			public static void Postfix(MinionVitalsPanel __instance)
			{
				__instance.AddAmountLine(BAmounts.Wet);
			}
		}
	}
}