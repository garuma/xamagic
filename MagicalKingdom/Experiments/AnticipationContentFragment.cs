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
using Android.Graphics;
using Android.Animation;

using Tau.Core;
using Tau.Core.Curves;
using Tau.Android;

namespace MagicalKingdom
{
	public class AnticipationContentFragment : Fragment, IExperiment
	{
		Tau.Core.ITimeCurve curve;

		View loading;
		ImageView pictureBox;
		View pictureFrame;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.AnticipationContent, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			loading = view.FindViewById (Resource.Id.pictureProgress);
			pictureBox = view.FindViewById<ImageView> (Resource.Id.picture);
			pictureFrame = view.FindViewById (Resource.Id.pictureCanvas);
		}

		public event Action<float> AnimationCurrentCompletion;

		public void StartAnimation (bool slowMode)
		{
			pictureBox.Visibility = ViewStates.Invisible;
			loading.Visibility = ViewStates.Visible;
			pictureFrame.RotationY = 0;

			var time = 2 * (slowMode ? AnimationConstants.SlowAnimationTime : AnimationConstants.NormalAnimationTime);
			var animator = ObjectAnimator.OfFloat (pictureFrame, "rotationY", 0, 180);
			animator.SetInterpolator (curve.ToAndroidInterpolator ());
			animator.SetDuration (time);
			animator.StartDelay = 2000;
			animator.SetAutoCancel (true);
			if (AnimationCurrentCompletion != null) {
				animator.AnimationEnd += (sender, e) => {
					AnimationCurrentCompletion (-1);
					((Animator)sender).RemoveAllListeners ();
				};
				animator.Update += (sender, e) => {
					if (e.Animation.AnimatedFraction >= .5f) {
						pictureBox.Visibility = ViewStates.Visible;
						loading.Visibility = ViewStates.Invisible;
					}
					AnimationCurrentCompletion (e.Animation.CurrentPlayTime / (float)time);
				};
			}
			animator.Start ();
		}

		public Tau.Core.ITimeCurve CreatePreferredTimeCurve ()
		{
			return new AnticipationCurve ();
		}

		public void SetTimeCurve (ITimeCurve curve)
		{
			this.curve = curve;
		}

		public string PreferredCurveName {
			get {
				return "Anticipation";
			}
		}
	}
}

