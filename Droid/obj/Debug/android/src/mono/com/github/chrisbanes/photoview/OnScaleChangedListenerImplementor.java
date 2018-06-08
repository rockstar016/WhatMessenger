package mono.com.github.chrisbanes.photoview;


public class OnScaleChangedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.github.chrisbanes.photoview.OnScaleChangedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onScaleChange:(FFF)V:GetOnScaleChange_FFFHandler:ImageViews.Photo.IOnScaleChangedListenerInvoker, PhotoView\n" +
			"";
		mono.android.Runtime.register ("ImageViews.Photo.IOnScaleChangedListenerImplementor, PhotoView", OnScaleChangedListenerImplementor.class, __md_methods);
	}


	public OnScaleChangedListenerImplementor ()
	{
		super ();
		if (getClass () == OnScaleChangedListenerImplementor.class)
			mono.android.TypeManager.Activate ("ImageViews.Photo.IOnScaleChangedListenerImplementor, PhotoView", "", this, new java.lang.Object[] {  });
	}


	public void onScaleChange (float p0, float p1, float p2)
	{
		n_onScaleChange (p0, p1, p2);
	}

	private native void n_onScaleChange (float p0, float p1, float p2);

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
