﻿using Beached.Content.Scripts.StarmapEntities;
using UnityEngine;

namespace Beached.Content.Defs.StarmapEntities
{
	/// <summary>
	/// This entity handles the actual animation of the meteor swarm, it is not selectable on the starmap
	/// </summary>
	public class MeteorSwarmVisualPOIConfig : IEntityConfig
	{
		public static string ID = "Beached_ClusterSwarmVisualPOI";

		public GameObject CreatePrefab()
		{
			var prefab = EntityTemplates.CreateEntity(ID, ID);

			prefab.AddOrGet<SaveLoadRoot>();
			prefab.AddOrGet<InvisibleClusterGridEntity>()._name = "Meteor Swarm Visualizer";

			return prefab;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}