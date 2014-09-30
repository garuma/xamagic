using System;
using System.Linq;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Tau.Core;
using Tau.Core.Curves;
using Tau.Android;

namespace MagicalKingdom
{
	[Activity (Label = "Xamagic", MainLauncher = true, Icon = "@drawable/icon",
	           ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape,
	           Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar.Fullscreen")]
	public class MainActivity : Activity
	{
		CheckBox slowMode;
		Button start;
		InterpolatorGraphView graph;

		Spinner experimentSpinner;
		Spinner curveName;
		IExperiment currentExperiment;
		string[] experimentNames;
		Action[] experimentSwitchers;
		ITimeCurve[] availableCurves;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			experimentNames = new string[] {
				"Mystery Experiment",
				"Anticipation",
				"Squash/Stretch"
			};
			experimentSwitchers = new Action[] {
				SwitchToExperiment<BounceMarkerContentFragment>,
				SwitchToExperiment<AnticipationContentFragment>,
				SwitchToExperiment<SquashStretchContentFragment>
			};
			availableCurves = new ITimeCurve[] {
				new LinearCurve (),
				new QuadraticCurve (),
				new InvertedQuadraticCurve (),
				new SineCurve ()
			};

			graph = FindViewById<InterpolatorGraphView> (Resource.Id.graphView);
			slowMode = FindViewById<CheckBox> (Resource.Id.slowMotion);
			start = FindViewById<Button> (Resource.Id.start);
			experimentSpinner = FindViewById<Spinner> (Resource.Id.experimentSpinner);
			curveName = FindViewById<Spinner> (Resource.Id.curveName);

			var adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleSpinnerItem, experimentNames);
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			experimentSpinner.Adapter = adapter;
			experimentSpinner.ItemSelected += (sender, e) => experimentSwitchers [e.Position] ();

			var curveNames = new System.Collections.Generic.List<string> { "Default" };
			curveNames.AddRange (availableCurves.Select (c => ToHumanString (c.GetType ().Name)));
			adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleSpinnerItem, curveNames.ToArray ());
			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			curveName.Adapter = adapter;
			curveName.ItemSelected += HandleCurveSelected;

			start.Click += (sender, e) => {
				if (currentExperiment == null)
					return;
				graph.SetAnimationCompletion (0);
				currentExperiment.AnimationCurrentCompletion += graph.SetAnimationCompletion;
				currentExperiment.StartAnimation (slowMode.Checked);
			};

			SwitchToExperiment<BounceMarkerContentFragment> ();
		}

		void HandleCurveSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			if (currentExperiment == null)
				return;

			ITimeCurve curve = null;
			if (e.Position == 0)
				curve = currentExperiment.CreatePreferredTimeCurve ();
			else
				curve = availableCurves [e.Position - 1];
			currentExperiment.SetTimeCurve (curve);
			graph.SetAnimationCompletion (0);
			graph.SetInterpolatorCurve (curve);
		}

		void SwitchToExperiment<TFragment> () where TFragment : Fragment, IExperiment, new()
		{
			var frag = new TFragment ();
			FragmentManager.BeginTransaction ()
				.Replace (Resource.Id.contentContainer, frag)
				.Commit ();
			FragmentManager.ExecutePendingTransactions ();
			currentExperiment = frag;
			var curve = currentExperiment.CreatePreferredTimeCurve ();
			currentExperiment.SetTimeCurve (curve);
			graph.SetAnimationCompletion (0);
			graph.SetInterpolatorCurve (curve);
		}

		string ToHumanString (string str)
		{
			var sb = new System.Text.StringBuilder (str.Length + 10);
			sb.Append (char.ToUpper (str [0]));
			for (int i = 1; i < str.Length; i++) {
				var chr = str[i];
				if (char.IsUpper (chr) && i < str.Length - 1 && (!char.IsUpper (str, i - 1) || !char.IsUpper (str, i+1)))
					sb.Append (' ');
				sb.Append (chr);
			}
			return sb.ToString ().Trim ();
		}
	}
}


