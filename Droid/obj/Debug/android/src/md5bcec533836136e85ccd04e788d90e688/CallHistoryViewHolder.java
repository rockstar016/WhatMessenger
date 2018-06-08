package md5bcec533836136e85ccd04e788d90e688;


public class CallHistoryViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.CallHistory.CallHistoryViewHolder, WhatMessenger.Droid", CallHistoryViewHolder.class, __md_methods);
	}


	public CallHistoryViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == CallHistoryViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.CallHistory.CallHistoryViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
