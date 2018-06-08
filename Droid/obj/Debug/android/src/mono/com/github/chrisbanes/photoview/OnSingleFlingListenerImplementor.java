package mono.com.github.chrisbanes.photoview;


public class OnSingleFlingListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.github.chrisbanes.photoview.OnSingleFlingListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onFling:(Landroid/view/MotionEvent;Landroid/view/MotionEvent;FF)Z:GetOnFling_Landroid_view_MotionEvent_Landroid_view_MotionEvent_FFHandler:ImageViews.Photo.IOnSingleFlingListenerInvoker, PhotoView\n" +
			"";
		mono.android.Runtime.register ("ImageViews.Photo.IOnSingleFlingListenerImplementor, PhotoView", OnSingleFlingListenerImplementor.class, __md_methods);
	}


	public OnSingleFlingListenerImplementor ()
	{
		super ();
		if (getClass () == OnSingleFlingListenerImplementor.class)
			mono.android.TypeManager.Activate ("ImageViews.Photo.IOnSingleFlingListenerImplementor, PhotoView", "", this, new java.lang.Object[] {  });
	}


	public boolean onFling (android.view.MotionEvent p0, android.view.MotionEvent p1, float p2, float p3)
	{
		return n_onFling (p0, p1, p2, p3);
	}

	private native boolean n_onFling (android.view.MotionEvent p0, android.view.MotionEvent p1, float p2, float p3);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
