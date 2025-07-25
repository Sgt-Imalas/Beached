﻿using Beached.Content.Scripts;
using Beached.Content.Scripts.Buildings;
using Beached.Content.Scripts.Entities;
using Beached.Content.Scripts.Entities.AI;
using Beached.Content.Scripts.Entities.Plant;
using Beached.Content.Scripts.Items;
using Database;

namespace Beached.Content.ModDb
{
	public class BStatusItems
	{
		// StatusItem.ALL_OVERLAYS

		public static StatusItem
			desiccation,
			secretingMucus,
			smoking,
			collectingRubber,
			collectingRubberHalted,
			collectingRubberFull,
			geneticallyMofidied,
			thawing,
			gunked,
			plushed,
			hunting,
			controllerByCollarDispenser,
			cultivatingGerms,
			meat,
			mirror,
			nonVega,
			iceWrathLashOut,
			sandboxCrumble,
			waterMillWattage;

		public const string
			ITEMS = "ITEMS",
			BUILDINGS = "BUILDINGS",
			CREATURES = "CREATURES",
			DUPLICANTS = "DUPLICANTS";

		[DbEntry]
		public static void RegisterMiscStatusItems(MiscStatusItems __instance)
		{
			meat = __instance.Add(new StatusItem(
				"Beached_Meat",
				ITEMS,
				"beached_statusitem_meat",
				StatusItem.IconType.Custom,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false));

			sandboxCrumble = __instance.Add(new StatusItem(
				"Beached_SandBoxCrumble",
				BUILDINGS,
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false));

			sandboxCrumble.SetResolveStringCallback(SandBox.ResolveStatusItemString);

			nonVega = __instance.Add(new StatusItem(
				"Beached_NonVega",
				ITEMS,
				"beached_statusitem_nonvega",
				StatusItem.IconType.Custom,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false));

			gunked = __instance.Add(new(
				"Beached_Gunked",
				BUILDINGS,
				string.Empty,
				StatusItem.IconType.Exclamation,
				NotificationType.Bad,
				false,
				OverlayModes.None.ID));

			cultivatingGerms = __instance.Add(new(
				"Beached_CultivatingGerms",
				CREATURES,
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Good,
				false,
				OverlayModes.None.ID));

			cultivatingGerms.SetResolveStringCallback(GermCultivator.ResolveStatusItemString);

			smoking = __instance.Add(new(
				"Beached_Smoking",
				CREATURES,
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID));

			smoking.SetResolveStringCallback((str, data) => data is SmokeCookable smokable ? smokable.GetStatusItemTooltip(str) : str);
		}

		[DbEntry]
		public static void RegisterDuplicantStatusItems(DuplicantStatusItems __instance)
		{
			thawing = __instance.Add(new StatusItem(
				"Beached_Thawing",
				DUPLICANTS,
				"",
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				status_overlays: (int)StatusItem.StatusItemOverlays.None));

			iceWrathLashOut = __instance.Add(new StatusItem(
				"Beached_IceWrathLashOut",
				DUPLICANTS,
				"",
				StatusItem.IconType.Exclamation,
				NotificationType.Bad,
				false,
				OverlayModes.None.ID,
				status_overlays: (int)StatusItem.StatusItemOverlays.None));
		}

		private static StatusItem SimpleBuildingStatus(BuildingStatusItems __instance, string id, NotificationType notificationType = NotificationType.Neutral)
		{
			return __instance.Add(new StatusItem(
				id,
				BUILDINGS,
				string.Empty,
				notificationType == NotificationType.Good || notificationType == NotificationType.Neutral ? StatusItem.IconType.Info : StatusItem.IconType.Exclamation,
				notificationType,
				false,
				OverlayModes.None.ID,
				false));
		}

		[DbEntry]
		public static void RegisterBuildingStatusItems(BuildingStatusItems __instance)
		{
			collectingRubber = SimpleBuildingStatus(__instance, "Beached_CollectingRubber");
			collectingRubber.resolveTooltipCallback += RubberTappable.ResolveStatusItemString;

			collectingRubberHalted = SimpleBuildingStatus(__instance, "Beached_CollectingRubberHalted", NotificationType.BadMinor);
			collectingRubberFull = SimpleBuildingStatus(__instance, "Beached_CollectingRubberFull");

			waterMillWattage = __instance.CreateStatusItem(
				"Beached_WaterMillWattage",
				BUILDINGS,
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				allow_multiples: false,
				OverlayModes.Power.ID);

			waterMillWattage.resolveStringCallback = (str, data) =>
			{
				return data is WaterGenerator generator
					? str.Replace("{Wattage}", GameUtil.GetFormattedWattage(generator.GetPower()))
					: str;
			};

			plushed = new(
				"Beached_Plushed",
				BUILDINGS,
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Good,
				false,
				OverlayModes.None.ID);

			plushed.SetResolveStringCallback(Beached_PlushiePlaceable.GetStatusItemTooltip);

			__instance.Add(plushed);
		}

		[DbEntry]
		public static void RegisterCreatureStatusItems(CreatureStatusItems __instance)
		{
			desiccation = new(
				"Beached_Desiccation",
				CREATURES,
				string.Empty,
				StatusItem.IconType.Exclamation,
				NotificationType.Bad,
				false,
				OverlayModes.None.ID);

			desiccation.SetResolveStringCallback((str, data) => data is MoistureMonitor.Instance moistureMonitor ? string.Format(str, moistureMonitor.timeUntilDeath) : str);

			__instance.Add(desiccation);

			mirror = __instance.Add(new StatusItem(
				"Beached_Mirror",
				CREATURES,
				"beached_statusitem_meat",
				StatusItem.IconType.Custom,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false));

			__instance.Add(mirror);


			geneticallyMofidied = new(
				"Beached_GeneticallyModified",
				CREATURES,
				"status_item_unknown_mutation",
				StatusItem.IconType.Custom,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID);

			geneticallyMofidied.SetResolveStringCallback((str, data) =>
			{
				return data is Beached_GeneticallyModifiableEgg egg ? egg.GetStatusItemString(str) : str;
			});

			__instance.Add(geneticallyMofidied);

			secretingMucus = new(
				"Beached_SecretingMucus",
				CREATURES,
				string.Empty,
				StatusItem.IconType.Exclamation,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false);

			__instance.Add(secretingMucus);

			hunting = new(
				"Beached_Hunting",
				CREATURES,
				string.Empty,
				StatusItem.IconType.Exclamation,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false);

			__instance.Add(hunting);

			controllerByCollarDispenser = new(
				"Beached_ControllerByCollarDispenser",
				CREATURES,
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.None.ID,
				false);

			controllerByCollarDispenser.SetResolveStringCallback(GetCollarDispenserString);

			__instance.Add(hunting);
		}

		private static string GetCollarDispenserString(string str, object data)
		{
			return data is CollarDispenser dispenser ? dispenser.FormatStatusItemString(str) : str;
		}
	}
}
