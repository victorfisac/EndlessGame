using System.IO;
using UnityEngine;


namespace EndlessGame.Plugins
{
    public static class SharePlugin
    {
        public const string SHARE_IMAGE_FILENAME = "share_image.png";


        public static void ShareDialog(string pMessage)
        {
            string _imagePath = Path.Combine(Application.persistentDataPath, SharePlugin.SHARE_IMAGE_FILENAME);

            #if !UNITY_EDITOR
            AndroidJavaClass _class = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject _object = new AndroidJavaObject("android.content.Intent");
            _object.Call<AndroidJavaObject>("setAction", _class.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass _uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject _uriObject = _uriClass.CallStatic<AndroidJavaObject>("parse", string.Concat("file://", _imagePath));
            _object.Call<AndroidJavaObject>("putExtra", _class.GetStatic<string>("EXTRA_STREAM"), _uriObject);

            _object.Call<AndroidJavaObject>("setType", "text/plain");
            _object.Call<AndroidJavaObject>("putExtra", _class.GetStatic<string>("EXTRA_TEXT"), pMessage);
            _object.Call<AndroidJavaObject>("putExtra", _class.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");

            _object.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass _unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject _activity = _unity.GetStatic<AndroidJavaObject>("currentActivity");

            _activity.Call("startActivity", _object);
            #else
            Debug.LogFormat("SharePlugin: starting share process with message '{0}'.", pMessage);
            #endif
        }
    }
}