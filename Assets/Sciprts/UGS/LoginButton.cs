using System;
using GameVanilla.Core;
using TMPro;
using UnityEngine;

public class LoginButton : MonoBehaviour
{
    [SerializeField] private SceneTransition translation;
    [SerializeField] private TextMeshProUGUI tmpButtonText;


    private void Start()
    {
        // loginButton.onClick.AddListener(LoginButtonPressed);
        LoginController.Instance.OnSignedIn += LoginController_OnSignedIn;
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
        if (!LoginController.Instance.IsLogin)
        {
            await LoginController.Instance.InitSignIn();  
        }
        else
        {
            translation.PerformTransition();
        }
            
        
    }
    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        translation.PerformTransition();
    }
}
