using System;
using GameVanilla.Core;
using TMPro;
using UnityEngine;

public class LoginButton : MonoBehaviour
{
    [SerializeField] private LoginController loginController;
    [SerializeField] private SceneTransition translation;
    [SerializeField] private TextMeshProUGUI tmpButtonText;
    void Start()
    {
       
    }

    private void OnEnable()
    {
        // loginButton.onClick.AddListener(LoginButtonPressed);
        loginController.OnSignedIn += LoginController_OnSignedIn;
        // loginController.OnAvatarUpdate += LoginController_OnAvatarUpdate;
        // loginController.OnSignedOut += LoginController_OnSignedOut;
    }
    private void OnDisable()
    {
        // loginButton.onClick.RemoveListener(LoginButtonPressed);
        // loginController.OnSignedIn -= LoginController_OnSignedIn;
        // loginController.OnAvatarUpdate -= LoginController_OnAvatarUpdate;
        // loginController.OnSignedOut -= LoginController_OnSignedOut;
    }
    public async void LoginButtonPressed()
    {
        await loginController.InitSignIn();
    }
    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        translation.PerformTransition();
    }
}
