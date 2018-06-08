package md56d2368ad61df4aba684cf168e3e321b4;


public class CallHistoryItemViewHolder
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.Fragments.Adapters.CallHistoryItemViewHolder, WhatMessenger.Droid", CallHistoryItemViewHolder.class, __md_methods);
	}


	public CallHistoryItemViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == CallHistoryItemViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.Fragments.Adapters.CallHistoryItemViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
