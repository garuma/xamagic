
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Animation;

using Tau.Android;

namespace MagicalKingdom
{
	public class BounceMarkerContentFragment : Fragment, IExperiment
	{
		ImageView marker;
		Tau.Core.ITimeCurve curve;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.BounceMarkerContent, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
			marker = view.FindViewById<ImageView> (Resource.Id.mapPin);
		}

		public event Action<float> AnimationCurrentCompletion;

		public void StartAnimation (bool slowMode)
		{
			var time = slowMode ? AnimationConstants.SlowAnimationTime : AnimationConstants.NormalAnimationTime;
			var interpolator = curve.ToAndroidInterpolator ();
			var animator = ObjectAnimator.OfFloat (marker, "translationY",
			                                       -(View.Height / 3 + marker.Height / 2),
			                                       0);
			animator.SetAutoCancel (true);
			animator.SetInterpolator (interpolator);
			animator.SetDuration (time);
			if (AnimationCurrentCompletion != null) {
				animator.AnimationEnd += (sender, e) => {
					AnimationCurrentCompletion (-1);
					((Animator)sender).RemoveAllListeners ();
				};
				animator.Update += (sender, e) => AnimationCurrentCompletion (e.Animation.CurrentPlayTime / (float)time);
			}
			animator.Start ();
		}

		public Tau.Core.ITimeCurve CreatePreferredTimeCurve ()
		{
			return new Tau.Core.Curves.BounceCurve ();
		}

		public void SetTimeCurve (Tau.Core.ITimeCurve curve)
		{
			this.curve = curve;
		}

		public string PreferredCurveName {
			get {
				return "Mysterious";
			}
		}
	}
}

