using System;
using Android.Content;
using Android.Database;

namespace Rock.Utils
{
    public class FileUtils
    {
		public static string GetPathToImage(Context context, Android.Net.Uri uri)
		{
            string path = "";
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using(ICursor cursor = context.ContentResolver.Query(uri, projection, null, null, null))
            {
                if(cursor != null)
                {
					int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
					cursor.MoveToFirst();
					path = cursor.GetString(columnIndex);
                }
            }
            return path;
		}
    }
}
