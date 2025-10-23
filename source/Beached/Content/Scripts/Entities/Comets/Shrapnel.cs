using HarmonyLib;

namespace Beached.Content.Scripts.Entities.Comets
{
	public class Shrapnel : Comet
	{
		private HashedString _FLYING_SOUND_ID_PARAMETER = "meteorType";

		//private static AccessTools.FieldRef<Comet, float> explosionMass;

		public void SetExplosionMass(float value)
		{
			explosionMass = value;
		}

		public override void OnSpawn()
		{
			StartLoopingSound2();
		}

		private void StartLoopingSound2()
		{
			loopingSounds.StartSound(flyingSound);
			loopingSounds.UpdateFirstParameter(flyingSound, _FLYING_SOUND_ID_PARAMETER, flyingSoundID);
		}
	}
}
