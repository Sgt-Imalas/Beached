﻿using Beached.Content.Scripts;
using Beached.Content.Scripts.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Beached.Content.Defs.Entities
{
	public class GlaciersConfig : IMultiEntityConfig
	{
		public const float WEIGHT_PER_TILE = 30;

		public const string
			MUFFINS = "Beached_Glacier_Muffins";

		public List<GameObject> CreatePrefabs() =>
		[
			CreateMuffins()
		];

		private static GameObject CreateMuffins()
		{
			var prefab = CreatePrefab(MUFFINS, 7, 3, "beached_glacier_muffins_kanim", [SleepingMuffinsConfig.ID]);

			prefab.AddOrGetDef<GlacierStoryTraitInitializer.Def>();
			return prefab;
		}

		private static GameObject CreatePrefab(string id, int width, int height, string anim, Tag[] rewardDrops = null, Vector3 rewardDropOffset = default)
		{
			var name = Strings.Get($"STRINGS.ENTITIES.GLACIERS.{id.ToUpperInvariant()}.NAME");
			var description = $"{Strings.Get($"STRINGS.ENTITIES.GLACIERS.{id.ToUpperInvariant()}.DESCRIPTION")}\n\n{STRINGS.ENTITIES.GLACIERS.GENERIC_THAW}";

			var prefab = EntityTemplates.CreatePlacedEntity(
				id,
				name,
				description,
				width * height * WEIGHT_PER_TILE,
				Assets.GetAnim(anim),
				"idle",
				Grid.SceneLayer.Ore,
				width,
				height,
				TUNING.DECOR.BONUS.TIER2,
				element: SimHashes.Ice,
				additionalTags:
				[
					BTags.glacier
				],
				defaultTemperature: MiscUtil.CelsiusToKelvin(-30));

			var glacier = prefab.AddComponent<Glacier>();
			glacier.rewards = rewardDrops;
			glacier.offset = rewardDropOffset;

			prefab.AddOrGet<KBatchedAnimHeatPostProcessingEffect>();

			prefab.AddOrGet<Meltable>().selfHeatKW = 960f;

			return prefab;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
