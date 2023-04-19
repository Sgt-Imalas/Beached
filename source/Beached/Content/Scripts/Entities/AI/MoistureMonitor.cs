﻿using Beached.Content.ModDb;
using Klei.AI;
using UnityEngine;

namespace Beached.Content.Scripts.Entities.AI
{
    public class MoistureMonitor : GameStateMachine<MoistureMonitor, MoistureMonitor.Instance, IStateMachineTarget, MoistureMonitor.Def>
    {
        private State wet;
        private DryStates dry;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = wet;

            wet
                .Enter(Moisturize)
                .Transition(dry.damp, Not(IsInLiquid));

            dry
                .DefaultState(dry.damp)
                .ToggleStateMachine(smi => new LubricatedMovementMonitor.Instance(smi.master))
                .ToggleAttributeModifier("DryingOut", smi => smi.baseMoistureModifier, null)
                .Transition(wet, IsInLiquid)
                .Transition(dry.desiccating, IsCompletelyDry)
                .EventTransition(ModHashes.producedLubricant, dry.damp);

            dry.damp
                .Enter(smi => smi.hasBeenDryFor = 0)
                .Enter(smi => SetSpeedModifier(smi, 0.66f))
                .UpdateTransition(dry.secreting, UpdateDrying);

            dry.secreting
                .ToggleBehaviour(BTags.Creatures.secretingMucus, CanProduceLubricant);

            dry.desiccating
                .Enter(smi => SetSpeedModifier(smi, 0.33f))
                .ToggleStatusItem(BStatusItems.desiccation, smi => smi)
                .Update(CheckDying)
                .Transition(wet, IsInLiquid);
        }


        private static void CheckDying(Instance smi, float dt)
        {
            smi.timeUntilDeath -= dt;

            if (!(smi.timeUntilDeath <= 0f)) return;

            var deathMonitor = smi.GetSMI<DeathMonitor.Instance>();
            deathMonitor?.Kill(BDeaths.desiccation);
            smi.Trigger((int)ModHashes.desiccated);
        }

        private static bool UpdateDrying(Instance smi, float dt)
        {
            smi.hasBeenDryFor += dt;
            smi.timeUntilDeath = smi.maxTimeUntilDeath;
            return smi.hasBeenDryFor >= 30f && smi.moisture.value < 80f;
        }

        private static bool CanProduceLubricant(Instance smi)
        {
            var cell = Grid.CellBelow(Grid.PosToCell(smi));
            return Grid.IsValidCell(cell) && Grid.IsSolidCell(cell);
        }

        public class DryStates : State
        {
            public State damp;
            public State dry;
            public State desiccating;
            public State secreting;
        }

        private static void SetSpeedModifier(Instance smi, float amount)
        {
            smi.navigator.defaultSpeed = smi.originalSpeed * amount;
        }

        private static void Moisturize(Instance smi)
        {
            smi.moisture.SetValue(100f);
            smi.timeUntilDeath = smi.maxTimeUntilDeath;
            smi.navigator.defaultSpeed = smi.originalSpeed;
        }

        private static bool IsCompletelyDry(Instance smi)
        {
            return smi.moisture.value <= 0;
        }

        private static bool IsInLiquid(Instance smi)
        {
            var cell = Grid.PosToCell(smi);
            return Grid.Element[cell].IsLiquid;
        }

        public class Def : BaseDef//, IGameObjectEffectDescriptor
        {
            internal float defaultDryRate = -30f / CONSTS.CYCLE_LENGTH;
            public SimHashes lubricant;
            public float lubricantMassKg;
            public float lubricantTemperatureKelvin;

            public override void Configure(GameObject prefab)
            {
                prefab.GetComponent<Modifiers>().initialAmounts.Add(BAmounts.Moisture.Id);
            }
        }

        public new class Instance : GameInstance
        {
            public AmountInstance moisture;
            public AttributeModifier baseMoistureModifier;
            public float originalSpeed;
            public Navigator navigator;
            public float hasBeenDryFor;
            public float timeUntilDeath;
            public float maxTimeUntilDeath = 60f;

            public Instance(IStateMachineTarget master, Def def) : base(master, def)
            {
                moisture = BAmounts.Moisture.Lookup(gameObject);
                moisture.value = moisture.GetMax();

                baseMoistureModifier = new AttributeModifier(
                    moisture.amount.deltaAttribute.Id,
                    def.defaultDryRate,
                    STRINGS.CREATURES.MODIFIERS.MOISTURE_LOSS_RATE.NAME);

                navigator = smi.GetComponent<Navigator>();
                originalSpeed = navigator.defaultSpeed;
            }

            public void ProduceLubricant()
            {
                BubbleManager.instance.SpawnBubble(
                    transform.GetPosition(),
                    Vector2.zero,
                    def.lubricant,
                    def.lubricantMassKg,
                    def.lubricantTemperatureKelvin);

                Trigger(ModHashes.producedLubricant);
            }
        }
    }
}
