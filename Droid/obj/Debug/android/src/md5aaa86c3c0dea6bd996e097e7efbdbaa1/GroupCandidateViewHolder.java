package md5aaa86c3c0dea6bd996e097e7efbdbaa1;


public class GroupCandidateViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.GroupDetail.Adapters.GroupCandidateViewHolder, WhatMessenger.Droid", GroupCandidateViewHolder.class, __md_methods);
	}


	public GroupCandidateViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == GroupCandidateViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.GroupDetail.Adapters.GroupCandidateViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
