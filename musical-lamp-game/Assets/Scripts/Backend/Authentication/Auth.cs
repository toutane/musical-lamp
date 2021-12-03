using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class User {
    public string playername;
    public string password;
}

public class Auth : MonoBehaviour
{
    public InputField tfPlayername, tfPassword;
    public Text SessionCookie;
    public Text ConnectionStatus;

    public bool isLogged;
    public void LoginUser() {
        if (isLogged) {
            Debug.Log("You are already logged... please logout");
        }
        else {
            if (!string.IsNullOrEmpty(tfPlayername.text) && !string.IsNullOrEmpty(tfPassword.text)) {
                StartCoroutine(SendLoginData());
            }
        }
    }

    public void Logout() {
        if (isLogged) {
                StartCoroutine(SendLogoutRequest());
        }
        else {
            Debug.Log("You must be logged-in for logout");
        }
    }

    public IEnumerator SendLogoutRequest() {
        var request = Webservices.Get("api/auth/logout");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
                Debug.LogError(request.error);
            }
            else {
                isLogged = false;
                Webservices.CookieString = null;
                ConnectionStatus.text = "Connection status: Not logged";
                SessionCookie.text = "Session cookie: ";
                Debug.Log(request.downloadHandler.text);
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
            isLogged = true;
            Webservices.CookieString = request.GetResponseHeader("set-cookie");
            SessionCookie.text = "Session cookie: " + Webservices.CookieString;
            ConnectionStatus.text = "Connection status: Logged";
            Debug.Log("Session cookie: " + Webservices.CookieString);
            Debug.Log(request.downloadHandler.text);
        }
    }

    public void Start() {
        if (!string.IsNullOrEmpty(Webservices.CookieString)) {
            isLogged = true;
            ConnectionStatus.text = "Connection status: Logged";
        }
        else {
            ConnectionStatus.text = "Connection status: Not logged";
        }
        SessionCookie.text = "Session cookie: " + Webservices.CookieString;
    }
}
