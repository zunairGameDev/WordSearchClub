using FunGames.UserConsent;
using UnityEngine;

public class FGDebugTrackingAutorizationExample : MonoBehaviour
{
    public void ShowATT()
    {
        FGUserConsent.ShowATT();
    }

    public void ShowGDPR()
    {
        FGUserConsent.ShowGDPR();
    }
}