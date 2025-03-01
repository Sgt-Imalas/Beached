﻿using Beached.Content.ModDb;
using KSerialization;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Beached.Content.Scripts.Entities.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class HarvestableCrystal : Workable, IApproachable
	{
		[MyCmpReq] private KSelectable kSelectable;
		[MyCmpReq] private KPrefabID kPrefabID;
		[MyCmpReq] private KBatchedAnimController kbac;
		[MyCmpReq] private GrowingCrystal growingCrystal;

		public bool allowMining = true;

		[Serialize] private bool markedForMining;
		[Serialize] public float crystalHealth;

		private Chore chore;

		[SerializeField] public float crystalMaxIntegrity = 100f;
		[SerializeField] public float crystalKgPerHarvest;
		[SerializeField] public SimHashes elementHarvested;

		private int currentBreakingStage = -1;

		public int GetHardness() => 100;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			workerStatusItem = Db.Get().DuplicantStatusItems.Digging;
			readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.DigRequiresSkillPerk;

			resetProgressOnStop = true;
			faceTargetWhenWorking = true;

			requiredSkillPerk = BSkillPerks.CAN_SAFELY_HARVEST_CLUSTERS_ID;
			shouldShowSkillPerkStatusItem = true;

			attributeConverter = Db.Get().AttributeConverters.DiggingSpeed; // TODO?
			attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;

			skillExperienceSkillGroup = BSkillGroups.PRECISION_ID;
			skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;

			multitoolContext = "dig";
			multitoolHitEffectTag = "fx_dig_splash";

			workingPstComplete = null;
			workingPstFailed = null;

			preferUnreservedCell = true;

			SetOffsetTable(OffsetGroups.InvertedStandardTable);

			Prioritizable.AddRef(gameObject);
		}

		public new int GetCell()
		{
			return Grid.PosToCell(this);
		}

		public override void OnSpawn()
		{
			Subscribe((int)GameHashes.Died, OnDeath);
			Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
			Subscribe((int)GameHashes.TagsChanged, OnTagsChanged);

			if (markedForMining)
			{
				Prioritizable.AddRef(gameObject);
			}

			UpdateStatusItem();
			UpdateChore();
			SetWorkTime(10f);
		}

		/*
        public override Vector3 GetTargetPoint()
        {
            var pos = GetComponent<KBoxCollider2D>()?.bounds.center ?? transform.GetPosition();
            pos.z = 0f;

            return pos;
        }
        */

		private void OnRefreshUserMenu(object data)
		{
			if (IsMineable())
			{
				if (!markedForMining)
				{
					var info = new KIconButtonMenu.ButtonInfo("action_uproot", global::STRINGS.UI.USERMENUACTIONS.DIG.NAME, () => MarkForDig(true), tooltipText: global::STRINGS.UI.USERMENUACTIONS.DIG.TOOLTIP);
					Game.Instance.userMenu.AddButton(gameObject, info);
				}
				else
				{
					var info = new KIconButtonMenu.ButtonInfo("icon_cancel", global::STRINGS.UI.USERMENUACTIONS.CANCELDIG.NAME, () => MarkForDig(false), tooltipText: global::STRINGS.UI.USERMENUACTIONS.CANCELDIG.TOOLTIP);
					Game.Instance.userMenu.AddButton(gameObject, info);
				}
			}
		}

		private void RefreshSymbols(float progress)
		{
			growingCrystal.shaftKbac.SetBlendValue(progress);
		}

		private void MarkForDig(bool dig)
		{
			MarkForDig(dig, new PrioritySetting(PriorityScreen.PriorityClass.basic, 5));
		}

		private void MarkForDig(bool mark, PrioritySetting priority)
		{
			mark = mark && IsMineable();

			if (markedForMining && !mark)
			{
				Prioritizable.RemoveRef(gameObject);
			}
			else if (!markedForMining && mark)
			{
				Prioritizable.AddRef(gameObject);
				if (TryGetComponent(out Prioritizable prioritizable))
				{
					prioritizable.SetMasterPriority(priority);
				}
			}

			markedForMining = mark;
			UpdateStatusItem();
			UpdateChore();
		}

		public bool IsMineable()
		{
			return growingCrystal.IsGrown();
		}

		private void OnTagsChanged(object obj)
		{
			MarkForDig(markedForMining);
		}

		private void OnDeath(object obj)
		{
			allowMining = false;
			markedForMining = false;
			UpdateChore();
		}

		private void UpdateStatusItem()
		{
			shouldShowSkillPerkStatusItem = markedForMining;
			base.UpdateStatusItem(null);

			if (markedForMining)
			{
				kSelectable.AddStatusItem(Db.Get().MiscStatusItems.WaitingForDig);
			}
			else
			{
				kSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.WaitingForDig, false);
			}
		}

		public override void OnStartWork(WorkerBase worker)
		{
			kPrefabID.AddTag(BTags.Creatures.beingMined);
		}

		public override void OnStopWork(WorkerBase worker)
		{
			kPrefabID.RemoveTag(BTags.Creatures.beingMined);
		}

		public override void OnCompleteWork(WorkerBase worker)
		{
			Log.Debug("COMPLETE");

			//gameObject.GetSMI<ShellGrowthMonitor.Instance>().Mine();
			var length = Mathf.FloorToInt(growingCrystal.GetCurrentLength());
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
				{
					var item = MiscUtil.Spawn(elementHarvested.CreateTag(), gameObject);
					if (item.TryGetComponent(out PrimaryElement primaryElement) && TryGetComponent(out PrimaryElement myElement))
					{
						primaryElement.Mass = crystalKgPerHarvest / 100f;
						primaryElement.Temperature = myElement.Temperature;
						if (myElement.DiseaseIdx != byte.MaxValue)
							primaryElement.AddDisease(myElement.DiseaseIdx, myElement.diseaseCount / length, "mined");
					}
				}
			}

			Trigger(ModHashes.crystalHarvested);

			MarkForDig(false);
		}

		public override bool OnWorkTick(WorkerBase worker, float dt)
		{
			var damage = dt / workTime;
			ApplyDamage(damage);

			return base.OnWorkTick(worker, dt);
		}

		public void ApplyDamage(float damage)
		{
			crystalHealth -= damage * crystalMaxIntegrity;
			RefreshSymbols(crystalHealth / crystalMaxIntegrity);
		}

		private void UpdateChore()
		{
			if (markedForMining && chore == null)
			{
				chore = new WorkChore<HarvestableCrystal>(Db.Get().ChoreTypes.Dig, this, is_preemptable: true);
			}
			else
			{
				if (!markedForMining && chore != null)
				{
					chore.Cancel("not marked for mining");
					chore = null;
				}
			}
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			var descriptors = base.GetDescriptors(go);
			if (allowMining)
			{
				descriptors.Add(new Descriptor(STRINGS.UI.MINEABLE.TITLE, STRINGS.UI.MINEABLE.TOOLTIP));
			}

			return descriptors;
		}

	}
}
