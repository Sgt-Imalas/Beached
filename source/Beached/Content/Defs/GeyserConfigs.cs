﻿using Beached.Content.ModDb.Germs;
using Klei;
using System.Collections.Generic;

namespace Beached.Content.Defs
{
	public class GeyserConfigs
	{
		public const string
			AMMONIA_VENT = "Beached_Ammonia",
			BISMUTH_VOLCANO = "Beached_Bismuth",
			CORAL_REEF = "Beached_CoralReef",
			HELIUM_VENT = "Beached_Helium",
			SALT_VOLCANO = "Beached_Salt",
			MURKY_BRINE_GEYSER = "Beached_MurkyBrine";

		public static void GenerateConfigs(List<GeyserGenericConfig.GeyserPrefabParams> list)
		{
			if (list == null)
			{
				Log.Warning("geyser configs list is null");
				return;
			}

			list.Add(new GeyserGenericConfig.GeyserPrefabParams(
				"beached_salt_volcano_kanim",
				3,
				3,
				new GeyserConfigurator.GeyserType(
					SALT_VOLCANO,
					SimHashes.MoltenSalt,
					GeyserConfigurator.GeyserShape.Liquid,
					MiscUtil.CelsiusToKelvin(1050),
					1000f,
					2000f,
					500f,
					[],
					null,
					geyserTemperature: 263f),
				true));

			list.Add(new GeyserGenericConfig.GeyserPrefabParams(
				"beached_murkywater_geyser_kanim",
				4,
				2,
				new GeyserConfigurator.GeyserType(
					MURKY_BRINE_GEYSER,
					Elements.murkyBrine,
					GeyserConfigurator.GeyserShape.Liquid,
					263.15f,
					1000f,
					2000f,
					500f,
					[],
					null,
					geyserTemperature: 263f),
				true));


			list.Add(new GeyserGenericConfig.GeyserPrefabParams(
				"beached_helium_vent_kanim",
				2,
				4,
				new GeyserConfigurator.GeyserType(
					HELIUM_VENT,
					SimHashes.Helium,
					GeyserConfigurator.GeyserShape.Gas,
					MiscUtil.CelsiusToKelvin(450),
					70f,
					140f,
					5f,
					[],
					null,
					60f),
				true));


			list.Add(new GeyserGenericConfig.GeyserPrefabParams(
				"beached_ammonia_vent_kanim",
				2,
				4,
				new GeyserConfigurator.GeyserType(
					AMMONIA_VENT,
					Elements.ammonia,
					GeyserConfigurator.GeyserShape.Gas,
					333.15f,
					70f,
					140f,
					5f,
					[],
					null,
					60f),
				true));

			list.Add(new GeyserGenericConfig.GeyserPrefabParams(
				"beached_bismuth_volcano_kanim",
				3,
				3,
				new GeyserConfigurator.GeyserType(
					BISMUTH_VOLCANO,
					Elements.bismuthMolten,
					GeyserConfigurator.GeyserShape.Molten,
					2900f,
					200f,
					400f,
					150f,
					[],
					null,
					480f,
					1080f,
					1 / 60f,
					0.1f),
				true));

			if (Db.Get().Diseases.TryGet(BDiseases.plankton.id) == null)
			{
				Log.Warning("no plankton germs");
			}

			// coral reef, emits planktonous salt water. it's high max pressure allows it to function under water.
			list.Add(new GeyserGenericConfig.GeyserPrefabParams(
				"beached_coral_reef_kanim",
				2,
				3,
				new GeyserConfigurator.GeyserType(
					CORAL_REEF,
					SimHashes.SaltWater,
					GeyserConfigurator.GeyserShape.Liquid,
					GameUtil.GetTemperatureConvertedToKelvin(40, GameUtil.TemperatureUnit.Celsius),
					1000f,
					2000f,
					4000f,
					[],
					null)
				.AddDisease(new SimUtil.DiseaseInfo
				{
					idx = Db.Get().Diseases.GetIndex(PlanktonGerms.ID),
					count = 20000
				}), false));
		}
	}
}
