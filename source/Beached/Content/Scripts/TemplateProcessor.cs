﻿using System.Collections.Generic;
using UnityEngine;

namespace Beached.Content.Scripts
{
	public class TemplateProcessor : KMonoBehaviour
	{
		[SerializeField]
		public int biomeOverride;

		[SerializeField]
		public List<Vector2I> offsets;

		public override void OnSpawn()
		{
			if (offsets == null)
			{
				Log.Warning("Template Processor spawned without configured cells.");
				return;
			}

			var origin = Grid.PosToCell(this);

			foreach (var offset in offsets)
			{
				var cell = Grid.OffsetCell(origin, offset.x, offset.y);

				if (ModAssets.biomeOverrideLookup.TryGetValue(biomeOverride, out var zone))
				{
					Beached_Grid.Instance.zoneTypeOverrides[cell] = zone;
				}
			}

			Beached_Grid.Instance.RegenerateBackwallTexture();
		}
	}
}
