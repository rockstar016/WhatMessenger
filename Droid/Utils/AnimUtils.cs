using System;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Android.Views.Animations;

namespace Rock.Utils
{
    public class AnimUtils
    {
        public static void ShowAnimationWithAlpha(View view, Action action)
        {
            
                AlphaAnimation animation = new AlphaAnimation(0, 1);
                animation.Duration = 600;
                view.StartAnimation(animation);
            animation.AnimationEnd += (sender, e) => {
                if (action != null)
                    action.Invoke();
                Log.Debug("Animation", "Animation view finish");
            };
        }


        public static void HideAnimationWithAlpha(View view)
        {
            
                AlphaAnimation animation = new AlphaAnimation(1, 0);
                animation.Duration = 50;
                view.StartAnimation(animation);

        }
    }
}
