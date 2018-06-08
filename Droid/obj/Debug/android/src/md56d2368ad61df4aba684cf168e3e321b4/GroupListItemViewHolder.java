package md56d2368ad61df4aba684cf168e3e321b4;


public class GroupListItemViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.Fragments.Adapters.GroupListItemViewHolder, WhatMessenger.Droid", GroupListItemViewHolder.class, __md_methods);
	}


	public GroupListItemViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == GroupListItemViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.Fragments.Adapters.GroupListItemViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
