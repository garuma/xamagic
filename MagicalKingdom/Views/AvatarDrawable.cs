
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
using Android.Graphics.Drawables;

namespace MagicalKingdom
{
	public class AvatarDrawable : BitmapDrawable
	{
		Paint avatar;
		Paint stroke;

		public AvatarDrawable (Bitmap baseBitmap)
			: base (baseBitmap)
		{
			this.avatar = new Paint { AntiAlias = true };
			this.stroke = new Paint {
				AntiAlias = true,
				Color = Color.Rgb (220, 220, 220),
			};
		}

		protected override void OnBoundsChange (Rect bounds)
		{
			base.OnBoundsChange (bounds);
			var baseBitmap = Bitmap;
			var shader = new BitmapShader (baseBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
			var matrix = new Matrix ();
			matrix.SetScale (((float)bounds.Width ()) / baseBitmap.Width,
			                 ((float)bounds.Height ()) / baseBitmap.Height,
			                 0.0f, 0.0f);
			shader.SetLocalMatrix (matrix);
			avatar.SetShader (shader);
		}

		public override void Draw (Canvas canvas)
		{
			var hwidth = Bounds.Width () / 2;
			var hheight = Bounds.Height () / 2;
			var radius = hwidth;
			var delta = radius / 10;

			canvas.DrawCircle (hwidth, hheight, radius, stroke);
			canvas.DrawCircle (hwidth, hheight, radius - delta, avatar);
		}
	}
}

