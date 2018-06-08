package md529adf122d36fe8c73342b3c9945a31d1;


public class BlockedListItemViewHolder
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.Account.Adapters.BlockedListItemViewHolder, WhatMessenger.Droid", BlockedListItemViewHolder.class, __md_methods);
	}


	public BlockedListItemViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == BlockedListItemViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.Account.Adapters.BlockedListItemViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
