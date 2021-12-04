using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject IsLoggedUI;

    public GameObject AuthManager;
    public void Update() {
        if (AuthManager.GetComponent<Auth>().isLogged) {
            LoginPanel.SetActive(false);
            IsLoggedUI.SetActive(true);
        }
    }
}
