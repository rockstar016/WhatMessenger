package md5b38a6ffcb6edddc63f84bd53d62dbdc7;


public class MainBaseFragment
	extends android.support.v4.app.Fragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAttach:(Landroid/content/Context;)V:GetOnAttach_Landroid_content_Context_Handler\n" +
			"n_onDetach:()V:GetOnDetachHandler\n" +
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.Bases.MainBaseFragment, WhatMessenger.Droid", MainBaseFragment.class, __md_methods);
	}


	public MainBaseFragment ()
	{
		super ();
		if (getClass () == MainBaseFragment.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.Bases.MainBaseFragment, WhatMessenger.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onAttach (android.content.Context p0)
	{
		n_onAttach (p0);
	}

	private native void n_onAttach (android.content.Context p0);


	public void onDetach ()
	{
		n_onDetach ();
	}

	private native void n_onDetach ();

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
