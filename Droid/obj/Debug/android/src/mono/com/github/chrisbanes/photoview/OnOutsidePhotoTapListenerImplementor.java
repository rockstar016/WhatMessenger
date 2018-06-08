package mono.com.github.chrisbanes.photoview;


public class OnOutsidePhotoTapListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.github.chrisbanes.photoview.OnOutsidePhotoTapListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onOutsidePhotoTap:(Landroid/widget/ImageView;)V:GetOnOutsidePhotoTap_Landroid_widget_ImageView_Handler:ImageViews.Photo.IOnOutsidePhotoTapListenerInvoker, PhotoView\n" +
			"";
		mono.android.Runtime.register ("ImageViews.Photo.IOnOutsidePhotoTapListenerImplementor, PhotoView", OnOutsidePhotoTapListenerImplementor.class, __md_methods);
	}


	public OnOutsidePhotoTapListenerImplementor ()
	{
		super ();
		if (getClass () == OnOutsidePhotoTapListenerImplementor.class)
			mono.android.TypeManager.Activate ("ImageViews.Photo.IOnOutsidePhotoTapListenerImplementor, PhotoView", "", this, new java.lang.Object[] {  });
	}


	public void onOutsidePhotoTap (android.widget.ImageView p0)
	{
		n_onOutsidePhotoTap (p0);
	}

	private native void n_onOutsidePhotoTap (android.widget.ImageView p0);

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
