﻿using Beached.Content.ModDb;
using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Beached.Content.Defs.Equipment
{
	public class ZeolitePendantConfig : IEquipmentConfig
	{
		public const string ID = "Beached_Equipment_ZeolitePendant";

		public EquipmentDef CreateEquipmentDef()
		{
			var attributeModifiers = new List<AttributeModifier>
			{
				new(TUNING.EQUIPMENT.ATTRIBUTE_MOD_IDS.DECOR, 20),
				new(Db.Get().Amounts.Stress.deltaAttribute.Id, -2f / CONSTS.CYCLE_LENGTH),
			};

			var equipmentDef = EquipmentTemplates.CreateEquipmentDef(
				ID,
				BAssignableSlots.JEWELLERY_ID,
				Elements.zeolite,
				30f,
				TUNING.EQUIPMENT.VESTS.WARM_VEST_ICON0,
				"necklace",
				"beached_zeolite_necklace_kanim",
				4,
				attributeModifiers,
				additional_tags:
				[
					GameTags.PedestalDisplayable
				]);

			return equipmentDef;
		}

		public void DoPostConfigure(GameObject go) { }

		public string[] GetDlcIds() => null;
	}
}
