﻿using HarmonyLib;
using UnityEngine;
using static ProcGen.SubWorld;

namespace Beached.Content.Scripts
{
	public class DepthsVeil : KMonoBehaviour
	{
		public static DepthsVeil Instance;

		private GameObject veil;
		private Material material;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Instance = null;
		}

		[HarmonyPatch(typeof(SubworldZoneRenderData), "OnActiveWorldChanged")]
		public class SubworldZoneRenderData_GenerateTexture_Patch
		{
			public static void Postfix(Texture2D ___indexTex)
			{
				if (Instance != null && Instance.material != null)
				{
					Instance.material.SetTexture("_ZoneTypeTex", ___indexTex);

					var color = ___indexTex.GetPixel(0, 0);
				}
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			/*			veil = Instantiate(ModAssets.Fx.darkVeilOverlay);
						material = veil.GetComponent<MeshRenderer>().material;

						veil.transform.localScale = new Vector3(Grid.WidthInMeters, Grid.HeightInMeters, 1);
						veil.transform.position = new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, Grid.GetLayerZ(Grid.SceneLayer.FXFront2));

						material.renderQueue = RenderQueues.WorldTransparent;
						material.SetTexture("_ZoneTypeTex", World.Instance.zoneRenderData.indexTex);
						material.SetColor("_OverlayColor", new Color(0.2f, 0.2f, 0.3f));

						veil.SetActive(true);*/
		}

		public void SetZoneType(ZoneType zoneType)
		{
			var indices = World.Instance.zoneRenderData.zoneTextureArrayIndices;
			var zone = (int)zoneType;

			if (indices.Length <= zone)
			{
				Log.Debug($"no such zone index {zoneType} ({zone}");
				return;
			}

			material.SetInt("_DepthsIndex", indices[zone]);
		}
	}
}
