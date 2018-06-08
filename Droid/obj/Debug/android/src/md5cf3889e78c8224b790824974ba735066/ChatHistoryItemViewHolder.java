package md5cf3889e78c8224b790824974ba735066;


public class ChatHistoryItemViewHolder
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.Fragments.ChatHistoryItemViewHolder, WhatMessenger.Droid", ChatHistoryItemViewHolder.class, __md_methods);
	}


	public ChatHistoryItemViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == ChatHistoryItemViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.Fragments.ChatHistoryItemViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
