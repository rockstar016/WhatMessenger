package md58708c25fbd4b8b28be9e0f4f8ee7e157;


public class GroupReceiveImageViewHolder
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.ChatDetailView.GroupReceiveImageViewHolder, WhatMessenger.Droid", GroupReceiveImageViewHolder.class, __md_methods);
	}


	public GroupReceiveImageViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == GroupReceiveImageViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.ChatDetailView.GroupReceiveImageViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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