using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Beached.Content.Defs.Buildings
{
	internal class VetTableConfig : IBuildingConfig
	{
		public const string ID = "Beached_VetTable";

		public override BuildingDef CreateBuildingDef()
		{
			var def = BuildingTemplates.CreateBuildingDef(
				ID,
				2,
				3,
				"vettable_kanim",
				BUILDINGS.HITPOINTS.TIER1,
				BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
				BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
				MATERIALS.ALL_METALS,
				BUILDINGS.MELTING_POINT_KELVIN.TIER1,
				BuildLocationRule.OnFloor,
				DECOR.NONE,
				NOISE_POLLUTION.NOISY.TIER1);

			def.ViewMode = OverlayModes.Rooms.ID;
			def.Overheatable = false;
			def.AudioCategory = "Metal";
			def.AudioSize = "large";
			def.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));

			return def;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType);
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGet<LoopingSounds>();

		}
	}
}
