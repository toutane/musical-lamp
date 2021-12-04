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
    public InputField tfPlayername, tfPassword, registerPlayername, registerPassword;
    public Text SessionCookie;
    public Text ConnectionStatus;
    public Text PlayerDataText;

    public GameObject MenuManager;

    public bool isLogged;

    public void RegisterNewPlayer() {
        if (!isLogged) {
            StartCoroutine(RegisterNewPlayerRequest());
        }
        else {
            Debug.Log("You must be logged out for register a new player!");
        }
    }
    public void GetPlayerData() {
        if (isLogged) {
            StartCoroutine(GetPlayerDataRequest());
        }
        else {
            Debug.Log("You be logged in first!");
        }
    }
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

    public IEnumerator GetPlayerDataRequest() {
        var request = Webservices.Get("api/player/data");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.LogError(request.error);
        }
        else {
            PlayerDataText.text = "Player data:\n" + request.downloadHandler.text;
            Debug.Log(request.downloadHandler.text);
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
            MenuManager.GetComponent<MenuManager>().IsLoggedUI.SetActive(false);
            MenuManager.GetComponent<MenuManager>().LoginPanel.SetActive(true);
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
            GetPlayerData();
        }
    }

    private IEnumerator RegisterNewPlayerRequest() {
        var player = new User {
            playername = registerPlayername.text,
            password = registerPassword.text
        };
        // Delete cookie before requesting a new register
        Webservices.CookieString = null;

        var request = Webservices.Post("api/auth/register", JsonUtility.ToJson(player));
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.LogError(request.error);
        }
        else {
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
