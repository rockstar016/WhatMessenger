package md5aaa86c3c0dea6bd996e097e7efbdbaa1;


public class GroupEditorItemViewHolder
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.GroupDetail.Adapters.GroupEditorItemViewHolder, WhatMessenger.Droid", GroupEditorItemViewHolder.class, __md_methods);
	}


	public GroupEditorItemViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == GroupEditorItemViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.GroupDetail.Adapters.GroupEditorItemViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
