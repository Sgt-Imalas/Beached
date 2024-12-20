﻿using Beached.Content.Defs.Comets;
using Database;
using Klei.AI;
using TUNING;

namespace Beached.Content.ModDb
{
	public class BGameplayEvents
	{
		public const string DIAMOND_SHOWER = "Beached_ClusterDiamondShowerEvent";
		public const string DIAMOND_METEOREVENTID = "Beached_Diamond";
		public static GameplayEvent ClusterDiamondShower;
		public const string ABYSSALITE_SHOWER = "Beached_ClusterAbyssaliteShowerEvent";
		public const string ABYSSALITE_METEOREVENTID = "Beached_Abyssalite";
		public static GameplayEvent ClusterAbyssaliteShower;

		[DbEntry]
		public static void Register(GameplayEvents __instance)
		{
			ClusterDiamondShower = __instance.Add(new MeteorShowerEvent(
				DIAMOND_SHOWER,
				300f,
				3.5f,
				METEORS.BOMBARDMENT_OFF.NONE,
				METEORS.BOMBARDMENT_ON.UNLIMITED,
				ClusterMapMeteorShowerConfig.GetFullID(DIAMOND_METEOREVENTID),
				true)
				.AddMeteor(DiamondCometConfig.ID, 1f)
				.AddMeteor(SparklingZirconCometConfig.ID, 0.001f)
				.AddMeteor(SparklingAquamarineCometConfig.ID, 0.001f)
				.AddMeteor(SparklingVoidCometConfig.ID, 0.001f));

			ClusterDiamondShower.tags.Add(BTags.wishingStars);

			ClusterAbyssaliteShower = __instance.Add(new MeteorShowerEvent(
				ABYSSALITE_SHOWER,
				300f,
				3.5f,
				METEORS.BOMBARDMENT_OFF.NONE,
				METEORS.BOMBARDMENT_ON.UNLIMITED,
				ClusterMapMeteorShowerConfig.GetFullID(ABYSSALITE_SHOWER),
				true)
				.AddMeteor(AbyssaliteCometConfig.ID, 1f));

		}
	}
}
