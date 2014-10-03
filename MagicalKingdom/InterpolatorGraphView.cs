
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Animation;
using Android.Views.Animations;

using Tau.Core;

namespace MagicalKingdom
{
	public class InterpolatorGraphView : View
	{
		bool layoutSet;
		ITimeCurve interpolator;
		float[] points;
		float completion;

		int x, y, size;

		Paint numberPaint, axisPaint, curvePaint;

		public InterpolatorGraphView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public InterpolatorGraphView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public InterpolatorGraphView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			axisPaint = new Paint {
				Color = Color.Rgb (0x73, 0x81, 0x82),
			};
			axisPaint.SetStyle (Paint.Style.Stroke);

			numberPaint = new Paint {
				AntiAlias = true,
				Color = Color.Rgb (0x2c, 0x3e, 0x50),
				TextSize = TypedValue.ApplyDimension (ComplexUnitType.Sp, 10, Metrics),
			};

			curvePaint = new Paint {
				AntiAlias = true,
				Color = Color.Rgb (0xb4, 0x55, 0xb6),
				StrokeWidth = TypedValue.ApplyDimension (ComplexUnitType.Dip, 1, Metrics),
			};
		}

		public void SetInterpolatorCurve (ITimeCurve interpolator)
		{
			this.interpolator = interpolator;
			if (layoutSet) {
				ComputeCurve (size, size, Precision);
				Invalidate ();
			}
		}

		public void SetAnimationCompletion (float completion)
		{
			this.completion = completion;
			Invalidate ();
		}

		DisplayMetrics Metrics {
			get { return Context.Resources.DisplayMetrics; }
		}

		int Precision {
			get { return (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, 1, Metrics); }
		}

		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout (changed, left, top, right, bottom);

			var width = Width - (PaddingRight + PaddingLeft);
			var height = Height - (PaddingBottom + PaddingTop);
			size = Math.Min (width, height);
			size = (size >> 2) << 2;

			x = (Width - size) / 2;
			y = PaddingTop;

			this.layoutSet = true;
			if (interpolator != null)
				ComputeCurve (size, size, Precision);
		}

		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			DrawAxis (canvas, x, y, size, size);
			DrawCurve (canvas, x, y, size, size, Precision);
		}

		void DrawAxis (Canvas canvas, int x, int y, int width, int height)
		{
			var margin = TypedValue.ApplyDimension (ComplexUnitType.Dip, 8, Metrics);
			canvas.DrawRect (x, y, x + width, y + height, axisPaint);
			canvas.DrawText ("0", x - margin, y + height + margin, numberPaint);
			canvas.DrawText ("1", x - margin, y, numberPaint);
			canvas.DrawText ("1", x + width + margin / 2, y + height + margin, numberPaint);
		}

		void DrawCurve (Canvas canvas, int x, int y, int width, int height, int precision)
		{
			if (points == null)
				return;

			canvas.Save ();
			if (completion >= 0)
				canvas.ClipRect (0, 0, x + completion * size, Height);
			canvas.Translate (x, y);
			canvas.DrawLines (points, curvePaint);
			canvas.Restore ();
		}

		void ComputeCurve (int width, int height, int precision)
		{
			var length = ((width / precision) - 1) * 4;
			if (points == null || points.Length != length)
				points = new float[length];

			var fwidth = (float)width;
			for (int seg = 0; seg < points.Length / 4; seg++) {
				var x1 = (seg * precision) / fwidth;
				var x2 = ((seg + 1) * precision) / fwidth;

				var index = seg * 4;
				points [index] = seg * precision;
				points [index + 1] = height - (float)(interpolator.Interpolate (x1) * height);
				points [index + 2] = (seg + 1) * precision;
				points [index + 3] = height - (float)(interpolator.Interpolate (x2) * height);
			}

			// Adjust end point to account for approximations
			points [length - 2] = width;
			var lastY = points [length - 1];
			points [length - 1] = lastY < height / 2 ? 0 : height;
		}
	}
}

