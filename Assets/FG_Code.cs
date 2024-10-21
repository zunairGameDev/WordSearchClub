using FunGames.Core;
using FunGames.Mediation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FG_Code : MonoBehaviour
{
    // Start is called before the first frame update


    void Awake()
    {
        //FunGamesSDK.Callbacks.Initialization += OnInitialization; // Triggered when module initialization start
        //FunGamesSDK.Callbacks.OnInitialized += OnInitialized; // Triggered when module initialization is completed
        //FunGamesSDK.Callbacks.OnAdsRemoved += OnAdsRemoved; // Triggered after ads were removed by RemoveAds()
    }
    private void OnInitialization()
    {
        // Do something when module start initializing.
        print("OnInitialization");
    }
    private void OnInitialized(bool success)
    {
        print("OnInitialized "+ success);
        // Do something when module initialization completed.
    }
    private void OnAdsRemoved()
    {
        // Do something when ads are removed.
    }
    private void ShowAd()
    {
        print("ShowAd");
        FGMediation.ShowInterstitial("Show Interstitial Ad", (success) => {
            Debug.Log("Show Interstitial Ad : " + success);
        });
    }
    private void OnDestroy()
    {
        FunGamesSDK.Callbacks.Initialization -= OnInitialization;
        FunGamesSDK.Callbacks.OnInitialized -= OnInitialized;
        FunGamesSDK.Callbacks.OnAdsRemoved -= OnAdsRemoved;
    }


    private void Update()
    {
        //if (Input.GetMouseButtonDown(0)) {
        //    ShowAd();
        //}
    }
}
