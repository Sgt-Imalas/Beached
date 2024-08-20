﻿using TUNING;
using UnityEngine;

namespace Beached.Content.Defs.Entities.Critters
{
	// Most basic form of a Slickshell, shared by adults, babies and all morphs
	public static class BaseSnailConfig
	{
		public static GameObject CreatePrefab(string id, string name, string desc, string anim_file, string traitId, string[] onDeathDrops)
		{
			var prefab = EntityTemplates.CreatePlacedEntity(
				id,
				name,
				desc,
				SlickShellTuning.MASS,
				Assets.GetAnim(anim_file),
				"idle_loop",
				Grid.SceneLayer.Creatures,
				1,
				1,
				DECOR.BONUS.TIER0);

			EntityTemplates.ExtendEntityToBasicCreature(
				prefab,
				FactionManager.FactionID.Friendly,
				traitId,
				CONSTS.NAV_GRID.WALKER_1X1,
				NavType.Floor,
				16,
				0.25f,
				SlickShellTuning.ON_DEATH_DROP,
				1,
				false,
				false,
				288.15f,
				343.15f,
				243.15f,
				373.15f);

			prefab.AddTag(GameTags.Creatures.Walker);
			prefab.AddTag(GameTags.Creatures.CrabFriend);
			prefab.AddTag(GameTags.Amphibious);

			prefab.GetComponent<Butcherable>().SetDrops(onDeathDrops);

			SnailBrain.ConfigureAI(prefab, null, BTags.Species.snail);

			return prefab;
		}
	}
}

