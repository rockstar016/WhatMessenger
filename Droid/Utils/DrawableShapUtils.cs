using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

namespace Rock.Utils
{
    public class DrawableShapUtils
    {
        public static ShapeDrawable DrawCirleWithBackGround(Context context, int width, int height, int color)
        {
            ShapeDrawable oval = new ShapeDrawable(new OvalShape());
            oval.SetIntrinsicHeight(height);
            oval.SetIntrinsicWidth(width);
            oval.Paint.Color = new Android.Graphics.Color(color);
            return oval;
        }
    }
}
