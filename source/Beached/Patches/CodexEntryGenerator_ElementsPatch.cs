﻿using Beached.Content;
using Beached.Content.Defs.Entities.Critters.Mites;
using Beached.Content.Defs.Flora;
using Beached.Content.Defs.Foods;
using Beached.Content.ModDb;
using Beached.Content.Scripts;
using Beached.Content.Scripts.Buildings;
using Beached.Content.Scripts.Entities.AI;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static Beached.Content.ModDb.LootTables;

namespace Beached.Patches
{
	public class CodexEntryGenerator_ElementsPatch
	{
		[HarmonyPatch(typeof(CodexEntryGenerator_Elements), "GetElementEntryContext")]
		public class CodexEntryGenerator_Elements_GetElementEntryContext_Patch
		{
			public static void Prefix(ref bool __state)
			{
				__state = CodexEntryGenerator_Elements.contextInstance == null;
			}

			public static void Postfix(ref CodexEntryGenerator_Elements.ElementEntryContext __result, ref bool __state)
			{
				if (!__state)
					return;

				AddPoffEntries(__result);
				AddLubricatableEntries(__result);
				AddSlagmiteMiningResults(__result, SlagmiteConfig.ID);
				AddSlagmiteMiningResults(__result, GleamiteConfig.ID);
				AdditionalEntries(__result);
			}

			private static void AdditionalEntries(CodexEntryGenerator_Elements.ElementEntryContext context)
			{
				List<(int order, System.Action fn)> entries = [];

				foreach (var prefab in Assets.Prefabs)
				{
					foreach (var entry in prefab.GetComponents<ICodexEntry>())
						entries.Add((entry.CodexEntrySortOrder(), () => entry.AddCodexEntries(context, prefab)));

					foreach (var entry in prefab.GetDefs<ICodexEntry>())
						entries.Add((entry.CodexEntrySortOrder(), () => entry.AddCodexEntries(context, prefab)));
				}

				entries
					.OrderBy(e => e.order)
					.Do(e => e.fn?.Invoke());
			}

			private static void AddSlagmiteMiningResults(CodexEntryGenerator_Elements.ElementEntryContext context, string critterId)
			{
				var prefab = Assets.GetPrefab(critterId);
				var shellMonitor = prefab.GetDef<ShellGrowthMonitor.Def>();

				if (shellMonitor == null)
					return;

				var autoMiner = Assets.GetPrefab(AutoMinerConfig.ID);

				var dropsTable = BDb.lootTables.Get(shellMonitor.lootTableId) as LootTable<MaterialReward>;
				var totalWeight = dropsTable.options.Sum(o => o.weight);

				foreach (var loot in dropsTable.options)
				{
					var conversionEntry = new CodexEntryGenerator_Elements.ConversionEntry()
					{
						title = autoMiner.GetProperName(),
						prefab = autoMiner,
						inSet = []
					};

					conversionEntry.inSet.Add(new ElementUsage(prefab.PrefabID(), 1f, false));
					context.usedMap.Add(prefab.PrefabID(), conversionEntry);

					var percentChance = loot.weight / totalWeight;
					var applyTo = new HashSet<ElementUsage>();
					var use = new ElementUsage(loot.item.tag, loot.item.mass, false)
					{
						customFormating = (tag, amount, continous) =>
						{
							var percentChanceStr = $"{STRINGS.CREATURES.SPECIES.BEACHED_SLAGMITE.ODDS} {GameUtil.GetStandardPercentageFloat(percentChance * 100f)}{(global::STRINGS.UI.UNITSUFFIXES.PERCENT)}";
							return $"{percentChanceStr}\n{GameUtil.GetFormattedMass(amount)}";
						}
					};

					conversionEntry.outSet.Add(use);
					context.madeMap.Add(loot.item.tag, conversionEntry);
				}
			}

			private static void AddLubricatableEntries(CodexEntryGenerator_Elements.ElementEntryContext __result)
			{
				foreach (var lubricant in Elements.lubricantStrengths.Keys)
				{
					var entry = __result.usedMap.map.GetOrAdd(lubricant.CreateTag(), () => []);

					foreach (var building in Assets.GetPrefabsWithComponent<Lubricatable>())
					{
						var applyTo = new HashSet<ElementUsage>();
						var lubricatable = building.GetComponent<Lubricatable>();
						var mucusUse = new ElementUsage(lubricant.CreateTag(), lubricatable.massPerUseOrPerSecond, true)
						{
							customFormating = (tag, amount, continous) => GetMucusFormatting(tag, amount, lubricatable)
						};

						applyTo.Add(mucusUse);

						entry.Add(new CodexEntryGenerator_Elements.ConversionEntry
						{
							title = building.GetProperName(),
							prefab = Assets.GetPrefab(building.PrefabID()),
							inSet = applyTo,
							outSet = []
						});
					}
				}
			}

			private static void AddPoffEntries(CodexEntryGenerator_Elements.ElementEntryContext __result)
			{
				var poffShroom = Assets.GetPrefab(PoffShroomConfig.ID);

				foreach (var poffEntry in PoffConfig.configs)
				{
					var elementTag = poffEntry.elementID.CreateTag();
					var entry = __result.usedMap.map.GetOrAdd(elementTag, () => []);
					var use = new ElementUsage(elementTag, PoffShroomConfig.GAS_CONSUMPTION_PER_SECOND_WILD, true)
					{
						customFormating = (tag, amount, continous) => GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.PerCycle)
					};

					var output = new ElementUsage(poffEntry.rawID, 1, true)
					{
						customFormating = (tag, amount, continous) => GameUtil.GetFormattedCaloriesForItem(poffEntry.rawID, 1f, GameUtil.TimeSlice.PerCycle)
					};

					var poff = Assets.GetPrefab(poffEntry.rawID);

					entry.Add(new CodexEntryGenerator_Elements.ConversionEntry
					{
						title = poffShroom.GetProperName(),
						prefab = poffShroom,
						inSet = [use],
						outSet = [output]
					});
				}
			}

			private static string GetMucusFormatting(Tag tag, float amount, Lubricatable lubricatable)
			{
				var mass = lubricatable.isTimedUse
					? GameUtil.GetFormattedMass(amount, GameUtil.TimeSlice.PerSecond)
					: GameUtil.GetFormattedMass(amount);

				return lubricatable.isTimedUse ? mass : $"{mass}/use";
			}
		}

	}
}
