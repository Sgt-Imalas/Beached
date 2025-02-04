﻿namespace Beached.Content.Scripts.Buildings
{
	public class DNAEggDeliverable : SingleEntityReceptacle
	{
		private KBatchedAnimTracker tracker;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			autoReplaceEntity = true;
			statusItemNeed = Db.Get().BuildingStatusItems.NeedEgg;
			synchronizeAnims = false;
			GetComponent<KBatchedAnimController>().SetSymbolVisiblity("egg_target", false);
		}
	}
}
