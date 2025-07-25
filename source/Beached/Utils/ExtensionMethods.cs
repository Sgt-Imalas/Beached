﻿using Beached.Content.Scripts.Buildings;
using Beached.Content.Scripts.ClassExtensions;
using Beached.Content.Scripts.Entities;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beached.Utils
{
	public static class ExtensionMethods
	{
		public static List<InterfaceType> GetDefs<InterfaceType>(this Component component) where InterfaceType : class
		{
			return component.gameObject.GetDefs<InterfaceType>();
		}

		public static List<InterfaceType> GetDefs<InterfaceType>(this GameObject gameObject) where InterfaceType : class
		{
			List<InterfaceType> result = [];

			if (gameObject.TryGetComponent(out StateMachineController smc))
			{
				if (!smc.defHandle.IsValid())
					return result;

				foreach (var def in smc.cmpdef.defs)
				{
					if (def is InterfaceType interfaceDef)
					{
						result ??= [];
						result.Add(interfaceDef);
					}
				}
			}

			return result;
		}

		public static void ConfigureCelsius(this TemperatureVulnerable instance, float tempWarningLow, float tempLethalLow, float tempWarningHigh, float tempLethalHigh)
		{
			instance.Configure(
				MiscUtil.CelsiusToKelvin(tempWarningLow),
				MiscUtil.CelsiusToKelvin(tempLethalLow),
				MiscUtil.CelsiusToKelvin(tempWarningHigh),
				MiscUtil.CelsiusToKelvin(tempLethalHigh));
		}

		public static bool TryGetReference<SpecifiedType>(this HierarchyReferences hierarchyReferences, string name, out SpecifiedType component) where SpecifiedType : Component
		{
			component = null;
			var references = hierarchyReferences.references;
			for (var i = 0; i < references.Length; i++)
			{
				var elementReference = references[i];
				if (elementReference.Name == name && elementReference.behaviour is SpecifiedType type)
				{
					component = type;
					return true;
				}
			}

			return false;
		}

		public static GameStateMachine<SMType, SMIType, MasterType, DefType>.State DebugStatusItem<SMType, SMIType, MasterType, DefType>(this GameStateMachine<SMType, SMIType, MasterType, DefType>.State state, object msg)
			where SMType : GameStateMachine<SMType, SMIType, MasterType, DefType>
			where SMIType : GameStateMachine<SMType, SMIType, MasterType, DefType>.GameInstance
			where MasterType : IStateMachineTarget
		{
#if DEBUG
			state.ToggleStatusItem(msg.ToString(), "");
#endif
			return state;
		}

		public static GameStateMachine<SMType, SMIType, MasterType, DefType>.State EnterTransition<SMType, SMIType, MasterType, DefType>(
			this GameStateMachine<SMType, SMIType, MasterType, DefType>.State state,
			Func<SMIType, GameStateMachine<SMType, SMIType, MasterType, DefType>.State> stateCallback)
		where SMType : GameStateMachine<SMType, SMIType, MasterType, DefType>
		where SMIType : GameStateMachine<SMType, SMIType, MasterType, DefType>.GameInstance
		where MasterType : IStateMachineTarget
		{
			state.Enter(smi =>
			{
				smi.GoTo(stateCallback(smi));
			});

			return state;
		}

		public static void AddTag(this Element element, Tag tag)
		{
			element.oreTags ??= [];
			element.oreTags = element.oreTags.AddToArray(tag);
		}

		public static Dictionary<MinionStartingStats, MinionStartingStatsExtension> minionStartingStatsExtensions = [];
		public static Dictionary<CavityInfo, CavityInfoExtension> cavityInfoExtensions = [];

		public static Direction Opposite(this Direction direction) => MiscUtil.GetOpposite(direction);

		public static void AddTags(this GameObject gameObject, params Tag[] tags)
		{
			if (gameObject == null)
				return;

			if (gameObject.TryGetComponent(out KPrefabID prefabID))
				foreach (var tag in tags)
				{
					prefabID.AddTag(tag);
				}
		}

		public static ValueType GetOrDefault<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, KeyType key, ValueType defaultValue)
		{
			return dictionary.TryGetValue(key, out var result) ? result : defaultValue;
		}

		public static ValueType GetOrAdd<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary, KeyType key, Func<ValueType> defaultValue)
		{
			if (dictionary.ContainsKey(key))
				return dictionary[key];

			dictionary[key] = defaultValue();
			return dictionary[key];
		}

		public static void AddTags(this KMonoBehaviour component, params Tag[] tags)
		{
			component.gameObject.AddTags(tags);
		}

		public static MinionStartingStatsExtension GetExtension(this MinionStartingStats stats)
		{
			if (!minionStartingStatsExtensions.ContainsKey(stats))
			{
				minionStartingStatsExtensions.Add(stats, new MinionStartingStatsExtension(stats));
			}

			return minionStartingStatsExtensions[stats];
		}

		public static Trait GetLifeGoalTrait(this MinionStartingStats stats)
		{
			return GetExtension(stats)?.lifeGoalTrait;
		}

		public static Dictionary<string, int> GetLifeGoalAttributes(this MinionStartingStats stats)
		{
			return GetExtension(stats).lifeGoalAttributes;
		}

		public static bool TryGetCollarDispenser(this CavityInfo cavity, out CollarDispenser dispenser)
		{
			dispenser = null;

			if (cavityInfoExtensions.TryGetValue(cavity, out var extension))
			{
				Log.Debug("there be dispensers " + extension.collarDispensers.Count);
				if (extension.collarDispensers.Count == 0)
					return false;

				dispenser = extension.collarDispensers[0];
				return true;

			}

			return false;
		}

		public static void AddCollarDispenser(this CavityInfo cavity, CollarDispenser collarDispenser)
		{
			Log.Debug("Added collar dispenser " + collarDispenser.GetProperName());

			var dispensers = cavityInfoExtensions
				.GetOrAdd(cavity, () => new CavityInfoExtension(cavity))
				.collarDispensers;

			if (dispensers.Contains(collarDispenser))
				return;

			dispensers.Add(collarDispenser);
		}

		public static void RemoveCollarDispenser(this CavityInfo cavity, CollarDispenser collarDispenser)
		{
			if (cavityInfoExtensions.TryGetValue(cavity, out var cavityInfoExtension))
				cavityInfoExtension.collarDispensers.Remove(collarDispenser);
		}

		public static void AddMirror(this CavityInfo cavity, Mirror mirror)
		{
			var mirrors = cavityInfoExtensions
				.GetOrAdd(cavity, () => new CavityInfoExtension(cavity))
				.mirrors;

			if (mirrors.Contains(mirror))
				return;

			mirrors.Add(mirror);
		}

		public static void RemoveMirror(this CavityInfo cavity, Mirror mirror)
		{
			if (cavityInfoExtensions.TryGetValue(cavity, out var cavityInfoExtension))
				cavityInfoExtension.mirrors.Remove(mirror);
		}

		public static int GetMirrorCount(this CavityInfo cavity)
		{
			return cavityInfoExtensions.TryGetValue(cavity, out var extension) ? extension.mirrors.Count : 0;
		}

		public static List<KPrefabID> GetNaturePOIs(this CavityInfo cavity)
		{
			return cavityInfoExtensions.TryGetValue(cavity, out var extension) ? extension.pois : null;
		}

		public static void AddNaturePOI(this CavityInfo cavity, KPrefabID kPrefabID)
		{
			Log.Debug("Added nature poi " + kPrefabID.GetProperName());

			cavityInfoExtensions
				.GetOrAdd(cavity, () => new CavityInfoExtension(cavity))
				.pois
				.Add(kPrefabID);
		}

		public static Direction RotateClockWise(this Direction direction)
		{
			return direction switch
			{
				Direction.Up => Direction.Right,
				Direction.Right => Direction.Down,
				Direction.Down => Direction.Left,
				Direction.Left => Direction.Up,
				_ => Direction.None,
			};
		}

		public static Direction RotateCounterClockWise(this Direction direction)
		{
			return direction switch
			{
				Direction.Up => Direction.Left,
				Direction.Right => Direction.Up,
				Direction.Down => Direction.Right,
				Direction.Left => Direction.Down,
				_ => Direction.None,
			};
		}

		public static CellOffset ToCellOffset(this Direction direction)
		{
			return direction switch
			{
				Direction.Up => CellOffset.up,
				Direction.Right => CellOffset.right,
				Direction.Down => CellOffset.down,
				Direction.Left => CellOffset.left,
				_ => CellOffset.none,
			};
		}

		public static Direction ToDirection(this CellOffset offset)
		{
			if (offset.x == offset.y)
				return Direction.None;

			if (offset.x > offset.y)
				return Direction.Right;

			if (offset.x < offset.y)
				return Direction.Left;

			if (offset.y > offset.x)
				return Direction.Up;

			return Direction.Down;
		}

		public static void RemoveNaturePOI(this CavityInfo cavity, KPrefabID kPrefabID)
		{
			if (cavityInfoExtensions.TryGetValue(cavity, out var cavityInfoExtension))
				cavityInfoExtension.pois.Remove(kPrefabID);
		}
	}
}
