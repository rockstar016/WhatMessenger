package md5c750e64b994dac0e1d4e0e6c0dfa5836;


public class ContactCandidateViewHolder
	extends md5b38a6ffcb6edddc63f84bd53d62dbdc7.BaseViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WhatMessenger.Droid.ContactDetail.Adapter.ContactCandidateViewHolder, WhatMessenger.Droid", ContactCandidateViewHolder.class, __md_methods);
	}


	public ContactCandidateViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == ContactCandidateViewHolder.class)
			mono.android.TypeManager.Activate ("WhatMessenger.Droid.ContactDetail.Adapter.ContactCandidateViewHolder, WhatMessenger.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
