﻿using HarmonyLib;
using System;
using System.Diagnostics;

namespace Beached
{
	public class Log
	{
		public static void Debug(object msg)
		{
			if (Mod.debugMode)
			{
				var stackTrace = new StackTrace();
				var name = stackTrace.GetFrame(1).GetMethod().FullDescription();
				global::Debug.Log($"[Beached]: {name} - {msg}");
			}
		}

		public static void Info(object msg)
		{
			global::Debug.Log($"[Beached]: {msg}");
		}

		public static void Assert(Func<bool> fn, object msg)
		{
			if (!fn())
				global::Debug.Log($"[Beached] [ASSERT FAILED]: {msg}");
		}

		public static void AssertNotNull(object obj, object objectName)
		{
			if (obj == null)
				global::Debug.Log($"[Beached] [ASSERT FAILED]: {objectName} is null");
		}

		public static void Warning(object msg)
		{
			global::Debug.Log($"[Beached] [WARNING]: {msg}");
		}

		public static void TranspilerIssue(object msg)
		{
			var stackTrace = new StackTrace();
			var name = stackTrace.GetFrame(1).GetMethod().FullDescription();

			global::Debug.Log($"[Beached] [TRANSPILER]: {name} - {msg}");
		}
	}
}
