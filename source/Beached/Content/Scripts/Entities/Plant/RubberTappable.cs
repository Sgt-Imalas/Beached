﻿using KSerialization;
using UnityEngine;

namespace Beached.Content.Scripts.Entities.Plant
{
	public class RubberTappable : StateMachineComponent<RubberTappable.StatesInstance>, ISidescreenButtonControl
	{
		[SerializeField] public string trackSymbol;
		[SerializeField] public float latexPerCycle;
		[SerializeField] public Storage latexStorage;
		[SerializeField] public SimHashes element;

		private KBatchedAnimController bucketKbac;

		[Serialize] public bool isTapped;
		[Serialize] public bool tapOrdered;

		public string SidescreenButtonText
		{
			get
			{
				if (isTapped)
					return "Remove Tap";

				if (tapOrdered)
					return "Cancel";

				return "Tap";
			}
		}

		public string SidescreenButtonTooltip => "Collect rubber from this tree.";

		public int ButtonSideScreenSortOrder() => 0;

		public int HorizontalGroupID() => -1;

		public void OnSidescreenButtonPressed()
		{
		}

		public override void OnSpawn() => smi.StartSM();

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
			throw new System.NotImplementedException();
		}

		public bool SidescreenButtonInteractable() => true;

		public bool SidescreenEnabled() => true;

		public class StatesInstance : GameStateMachine<States, StatesInstance, RubberTappable, object>.GameInstance
		{
			public KBatchedAnimController bucketKbac;
			public KBatchedAnimController kbac;
			public PrimaryElement primaryElement;
			public KBatchedAnimTracker tracker;
			public Storage latexStorage;
			public HashedString trackSymbol;
			public float latexPerSecond;
			public Element element;

			public StatesInstance(RubberTappable master) : base(master)
			{
				master.TryGetComponent(out KBatchedAnimController kbac);
				master.TryGetComponent(out PrimaryElement primaryElement);

				trackSymbol = master.trackSymbol;
				latexPerSecond = smi.master.latexPerCycle / CONSTS.CYCLE_LENGTH;
				latexStorage = master.latexStorage;
				element = ElementLoader.FindElementByHash(master.element);

				kbac.SetSymbolVisiblity(master.trackSymbol, false);
			}

			public void SetupBucket()
			{
				var gameObject = new GameObject("Beached_RubberBucket");
				gameObject.SetActive(false);

				var column = (Vector3)kbac
					.GetSymbolTransform(trackSymbol, out bool _)
					.GetColumn(3) with
				{
					z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront)
				};

				gameObject.transform.SetPosition(column);
				bucketKbac = gameObject.AddComponent<KBatchedAnimController>();
				bucketKbac.AnimFiles =
				[
					Assets.GetAnim( "beached_rubber_bucket_kanim")
				];

				tracker = gameObject.AddComponent<KBatchedAnimTracker>();
				tracker.symbol = trackSymbol;
				tracker.forceAlwaysVisible = true;

				tracker.SetAnimControllers(bucketKbac, kbac);

				bucketKbac.initialAnim = "collecting";

				kbac.SetSymbolVisiblity((KAnimHashedString)trackSymbol, false);
			}
		}

		public class States : GameStateMachine<States, StatesInstance, RubberTappable>
		{
			public State notTapped;
			public State tapOrdered;
			public CollectingStates collecting;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = notTapped;

				notTapped
					.Enter(DisableOverlay);

				collecting
					.Enter(EnableOverlay);

				collecting.blocked
					.EventTransition(GameHashes.WiltRecover, collecting)
					.ToggleStatusItem("Latex collection halted.", "");

				collecting.running
					.EventTransition(GameHashes.Wilt, collecting.blocked)
					.UpdateTransition(collecting.full, FillBucket, UpdateRate.SIM_1000ms)
					.ToggleStatusItem("Stored Latex {0}", "{0}", resolve_string_callback: (str, smi) => string.Format(str, smi.master.latexStorage.MassStored()));

				collecting.full
					.EventHandlerTransition(GameHashes.OnStorageChange, collecting.running, (smi, _) => !smi.latexStorage.IsFull())
					.ToggleStatusItem("Latex Bucket Full", "");
			}

			public class CollectingStates : State
			{
				public State running;
				public State blocked;
				public State full;
			}

			private bool FillBucket(StatesInstance smi, float dt)
			{
				if (smi.latexStorage.IsFull())
					return true;

				var material = smi.element.substance.SpawnResource(
					smi.transform.position,
					smi.latexPerSecond * dt,
					smi.primaryElement.Temperature,
					byte.MaxValue,
					0,
					true);

				smi.latexStorage.Store(material, true);

				return false;
			}

			private void EnableOverlay(StatesInstance smi)
			{
				if (smi.bucketKbac == null)
					smi.SetupBucket();

				smi.kbac.SetSymbolVisiblity(smi.master.trackSymbol, true);
				smi.master.bucketKbac.enabled = true;
			}

			private void DisableOverlay(StatesInstance smi)
			{
				if (smi.master.bucketKbac == null)
					return;

				smi.kbac.SetSymbolVisiblity(smi.master.trackSymbol, false);
				smi.master.bucketKbac.enabled = false;
			}
		}
	}
}
