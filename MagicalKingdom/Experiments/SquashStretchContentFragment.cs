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
	public class SquashStretchContentFragment : Fragment, IExperiment
	{
		ITimeCurve curve;
		TextView textBubble;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.SquashStretchContent, container, false);
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			var avatarImage = view.FindViewById<ImageView> (Resource.Id.avatarImage);
			var avatar = BitmapFactory.DecodeResource (Resources, Resource.Drawable.chris);
			avatarImage.SetImageDrawable (new AvatarDrawable (avatar));

			textBubble = view.FindViewById<TextView> (Resource.Id.textBubble);
		}

		public event Action<float> AnimationCurrentCompletion;

		public void StartAnimation (bool slowMode)
		{
			textBubble.Visibility = ViewStates.Visible;
			textBubble.PivotX = textBubble.Width / 2;
			textBubble.PivotY = 0;

			var fallTime = 300;
			var time = 600;

			if (slowMode) {
				fallTime *= 5;
				time *= 5;
			}

			var interpolator = curve.ToAndroidInterpolator ();

			var animator = ObjectAnimator.OfFloat (textBubble, "translationY", 200, 0);
			animator.SetAutoCancel (true);
			animator.SetDuration (fallTime);
			animator.SetInterpolator (new Android.Views.Animations.AccelerateInterpolator ());

			var squashStretch = ObjectAnimator.OfPropertyValuesHolder (textBubble,
			                                                           PropertyValuesHolder.OfFloat ("scaleY", 1f, 0.4f),
			                                                           PropertyValuesHolder.OfFloat ("scaleX", 1f, 1.1f));
			squashStretch.SetAutoCancel (true);
			squashStretch.SetDuration (time);
			squashStretch.SetInterpolator (interpolator);

			if (AnimationCurrentCompletion != null) {
				squashStretch.AnimationEnd += (sender, e) => {
					AnimationCurrentCompletion (-1);
					((Animator)sender).RemoveAllListeners ();
				};
				squashStretch.Update += (sender, e) => AnimationCurrentCompletion (e.Animation.CurrentPlayTime / (float)time);
			}

			var animSet = new AnimatorSet ();
			animSet.PlaySequentially (animator, squashStretch);
			animSet.Start ();
		}

		public ITimeCurve CreatePreferredTimeCurve ()
		{
			return new DoubleCriticalSpringDamperCurve ();
		}

		public void SetTimeCurve (ITimeCurve curve)
		{
			this.curve = curve;
		}

		public string PreferredCurveName {
			get {
				return "Squash/Stretch";
			}
		}
	}
}

