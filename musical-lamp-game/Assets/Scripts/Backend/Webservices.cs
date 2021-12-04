using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class Webservices : MonoBehaviour
{
    static string baseURL = "http://localhost:3000";

    public static string CookieString {
        get {
            return PlayerPrefs.GetString("cookie");
        }
        set {
            PlayerPrefs.SetString("cookie", value);
        }
    }

    public static string BuildUrl(string path) {
        return Path.Combine(baseURL, path).Replace(Path.DirectorySeparatorChar, '/');
    }

    public static UnityWebRequest Get(string path) {
        var request = new UnityWebRequest(BuildUrl(path), "GET");
        if (!string.IsNullOrEmpty(CookieString)) {
            request.SetRequestHeader("cookie", CookieString);
        }
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }

    public static UnityWebRequest Post(string path, string jsonString) {
        var request = new UnityWebRequest(BuildUrl(path), "POST");
        if (!string.IsNullOrEmpty(CookieString)) {
            request.SetRequestHeader("cookie", CookieString);
            Debug.Log("add cookie to the request");
        }
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        return request;
    }
}
