﻿using Beached.Content.DefBuilders;
using Beached.Content.Defs.Foods;
using Beached.Content.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Beached.Content.Defs.Entities.Critters.Muffins
{
	[EntityConfigOrder(CONSTS.CRITTER_LOAD_ORDER.BABY - 1)]
	public class MuffinConfig : BaseMuffinConfig, IEntityConfig
	{
		public const string ID = "Beached_Muffin";
		public const string EGG_ID = "Beached_MuffinEgg";
		protected override string AnimFile => "beached_muffin_kanim";

		protected override string Id => ID;

		public GameObject CreatePrefab() => CreatePrefab(this);

		protected override CritterBuilder ConfigureCritter(CritterBuilder builder)
		{
			return base.ConfigureCritter(builder)
				.Egg(BabyMuffinConfig.ID, "beached_egg_muffin_kanim")
					.Fertility(30)
					.Incubation(10)
					.EggChance(EGG_ID, 1)
					.Done()
				.Weapon()
					.Damage(2f, 3f)
					.Done()
				.ShedFur(0.25f, Util.ColorFromHex("d7dfed"))
				.Tag(GameTags.OriginalCreature);
		}

		public override GameObject CreatePrefab(BaseCritterConfig config)
		{
			var prefab = base.CreatePrefab(config);
			prefab.AddComponent<CollarWearer>();

			return prefab;
		}

		public static void OnPostEntitiesLoaded()
		{
			//Assets.GetPrefab(EGG_ID).AddTag(BTags.Creatures.doNotTargetMeByCarnivores);
			ConfigureDiet(Assets.GetPrefab(ID));
		}

		private static Diet ConfigureDiet(GameObject prefab)
		{
			HashSet<Tag> meats = [
				MeatConfig.ID,
				CookedMeatConfig.ID,
				FishMeatConfig.ID,
				SmokedFishConfig.ID,
				RawSnailConfig.ID,
				SmokedSnailConfig.ID
			];

			if (DlcManager.IsContentSubscribed(DlcManager.DLC4_ID))
			{
				meats.Add(DinosaurMeatConfig.ID);
				meats.Add(SmokedDinosaurMeatConfig.ID);
				meats.Add(PrehistoricPacuFilletConfig.ID);
				meats.Add(SmokedFish.ID);
			}

			var diet = new Diet(
				new Diet.Info(
					meats,
					Elements.bone.CreateTag(),
					400_000f),
				new Diet.Info(
					[RawEggConfig.ID, CookedEggConfig.ID, QuicheConfig.ID],
					null,
					calories_per_kg: 400_000));

			var creatureCalorieMonitor = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
			creatureCalorieMonitor.diet = diet;

			prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;

			return diet;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}
