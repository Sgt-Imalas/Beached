﻿using Beached.Content.Scripts;
using Klei.AI;
using System;
using UnityEngine;

namespace Beached.Content.Defs.StarmapEntities
{
	public class Drale_SpacedOutConfig : IEntityConfig, IHasDlcRestrictions
	{
		public const string ID = "Beached_Drale_SpacedOut";
		public const string TRAIT_ID = "Beached_Drale_SpacedOutOriginal";

		public GameObject CreatePrefab()
		{
			var name = "Space Drale";
			var prefab = EntityTemplates.CreateEntity(ID, name, true);

			prefab.AddOrGet<SaveLoadRoot>();
			prefab.AddOrGet<StateMachineController>();
			prefab.AddOrGet<Notifier>();
			prefab.AddOrGet<LoopingSounds>();

			prefab.AddOrGet<InfoDescription>().description = "Big whale boi";

			var destinationSelector = prefab.AddOrGet<ClusterDestinationSelector>();
			destinationSelector.assignable = false;
			destinationSelector.canNavigateFogOfWar = true;
			destinationSelector.requireAsteroidDestination = true;
			destinationSelector.requireLaunchPadOnAsteroidDestination = false;
			destinationSelector.dodgesHiddenAsteroids = true;

			var visualizer = prefab.AddOrGet<ClusterMapMeteorShowerVisualizer>();
			visualizer.p_name = name;
			visualizer.clusterAnimName = "beached_drale_kanim";
			visualizer.revealed = true;
			visualizer.forceRevealed = true;
			visualizer.isWorldEntity = true;

			var traveler = prefab.AddOrGet<ClusterTraveler>();
			traveler.revealsFogOfWarAsItTravels = true;
			traveler.peekRadius = 0;
			traveler.quickTravelToAsteroidIfInOrbit = false;

			var kPrefabId = prefab.AddOrGet<KPrefabID>();
			kPrefabId.AddTag(GameTags.Creature, false);

			prefab.AddOrGet<Traits>();

			var modifiers = prefab.AddOrGet<Modifiers>();

			var trait = Db.Get().CreateTrait(TRAIT_ID, name, name, null, false, null, true, true);
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 20000, name, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1000f / 600f, global::STRINGS.UI.TOOLTIPS.BASE_VALUE, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 3000f, name, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 1000f, name, false, false, true));

			modifiers.initialTraits.Add(TRAIT_ID);
			modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);

			var health = prefab.AddOrGet<Health>();
			health.isCritter = true;

			prefab.AddOrGetDef<Drale.Def>();

			return prefab;
		}

		public string[] GetForbiddenDlcIds() => null;

		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}

		[Obsolete]
		public string[] GetDlcIds() => null;
	}
}
