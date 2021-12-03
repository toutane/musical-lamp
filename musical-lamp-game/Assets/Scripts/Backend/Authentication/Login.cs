using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class User {
    public string playername;
    public string password;
}

public class Login : MonoBehaviour
{
   public InputField tfPlayername, tfPassword;

    public void LoginUser()
    {
        if (!string.IsNullOrEmpty(tfPlayername.text) && !string.IsNullOrEmpty(tfPassword.text))
        {
            StartCoroutine(SendLoginData());
        }
    }

    private IEnumerator SendLoginData()
    {
        var user = new User
        {
            playername = tfPlayername.text,
            password = tfPassword.text
        };
        // Delete cookie before requesting a new login
        Webservices.CookieString = null;

        var request = Webservices.Post("api/auth/login", JsonUtility.ToJson(user));
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Webservices.CookieString = request.GetResponseHeader("set-cookie");
            Debug.Log(Webservices.CookieString);
            Debug.Log(request.downloadHandler.text);
        }
    }
}
