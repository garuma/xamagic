using System;

namespace MagicalKingdom
{
	public interface IExperiment
	{
		event Action<float> AnimationCurrentCompletion;

		void StartAnimation (bool slowMode);

		Tau.Core.ITimeCurve CreatePreferredTimeCurve ();
		string PreferredCurveName { get; }
		void SetTimeCurve (Tau.Core.ITimeCurve curve);
	}
}

