package md58708c25fbd4b8b28be9e0f4f8ee7e157;


public class PdfFullScreenViewActivity
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.ChatDetailView.PdfFullScreenViewActivity, WhatMessenger.Droid", PdfFullScreenViewActivity.class, __md_methods);
	}


	public PdfFullScreenViewActivity ()
	{
		super ();
		if (getClass () == PdfFullScreenViewActivity.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.ChatDetailView.PdfFullScreenViewActivity, WhatMessenger.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
