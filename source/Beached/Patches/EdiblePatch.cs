﻿using Beached.Content;
using Beached.Content.Scripts;
using HarmonyLib;

namespace Beached.Patches
{
	public class EdiblePatch
	{
		[HarmonyPatch(typeof(Edible), nameof(Edible.AddOnConsumeEffects))]
		public class Edible_AddOnConsumeEffects_Patch
		{
			public static void Postfix(Edible __instance, Worker worker)
			{
				if (!__instance.HasTag(BTags.palateCleanserFood) || worker == null)
					return;

				if (worker.TryGetComponent(out Beached_MinionStorage minionStorage))
					minionStorage.OnPalateCleansed();
			}
		}
	}
}