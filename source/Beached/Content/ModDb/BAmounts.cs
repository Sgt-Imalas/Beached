﻿using Klei.AI;

namespace Beached.Content.ModDb
{
	public class BAmounts
	{
		public static Amount Moisture;
		public static Amount LimpetGrowth;
		public static Amount Wet;

		[DbEntry]
		public static void Register(Database.Amounts __instance)
		{
			Moisture = __instance.CreateAmount(
				"Beach_Moisture",
				0f,
				100f,
				false,
				Units.Flat,
				0.35f,
				true,
				"STRINGS.CREATURES",
				"ui_icon_stamina",
				"attribute_stamina",
				"mod_stamina");

			Moisture.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle, null));

			LimpetGrowth = __instance.CreateAmount(
				"Beach_LimpetGrowth",
				0f,
				100f,
				true,
				Units.Flat,
				0.1675f,
				true,
				"STRINGS.CREATURES",
				"ui_icon_scale_growth");

			LimpetGrowth.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));

			Wet = __instance.CreateAmount(
				"Beached_Wet",
				0f,
				100f,
				true,
				Units.Flat,
				0.1675f,
				true,
				"STRINGS.DUPLICANTS",
				"beached_amount_wet");

			Wet.SetDisplayer(new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
		}
	}
}