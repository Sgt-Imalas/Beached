using HarmonyLib;

namespace Beached.Patches
{
	public class ResearchEntryPatch
	{

		[HarmonyPatch(typeof(ResearchEntry), nameof(ResearchEntry.SetTech))]
		public class ResearchEntry_SetTech_Patch
		{
			public static void Postfix(ResearchEntry __instance)
			{
				int index = 1; //child 0 is the icon prefab

				var container = __instance.iconPanel.transform;
				foreach (TechItem unlockedItem in __instance.targetTech.unlockedItems)
				{
					if (!Game.IsCorrectDlcActiveForCurrentSave(unlockedItem))
						continue;
					var child = container.GetChild(index++);
					if (child == null)
						continue;
					if (!unlockedItem.Id.StartsWith("Beached_"))
						continue;

					if (!child.TryGetComponent<HierarchyReferences>(out var hr))
						continue;

					var dlcOverlay = hr.GetReference<KImage>("DLCOverlay");
					dlcOverlay.gameObject.SetActive(true);
					//dlcOverlay.sprite = Assets.GetSprite("beached_tech_banner");
					dlcOverlay.color = ModAssets.Colors.beached;

					if (child.TryGetComponent<ToolTip>(out var tt))
					{
						tt.toolTip = $"{unlockedItem.Name}\n{unlockedItem.description}\n\n{"This Content is added by Beached"}";
					}
				}
			}
		}
	}
}
