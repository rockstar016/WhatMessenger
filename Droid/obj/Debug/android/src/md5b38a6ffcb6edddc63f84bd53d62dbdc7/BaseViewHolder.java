package md5b38a6ffcb6edddc63f84bd53d62dbdc7;


public class BaseViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.Bases.BaseViewHolder, WhatMessenger.Droid", BaseViewHolder.class, __md_methods);
	}


	public BaseViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == BaseViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.Bases.BaseViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

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
