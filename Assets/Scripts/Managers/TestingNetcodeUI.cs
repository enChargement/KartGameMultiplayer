using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Debug.Log("connected host");
            Hide();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Debug.Log("connected client");
            Hide();
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
