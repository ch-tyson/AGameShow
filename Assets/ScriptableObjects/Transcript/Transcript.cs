using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Transcript", menuName = "Transcript")]
public class Transcript : ScriptableObject
{
    public string latestPlayerResponse = "";
    public string latestAvatarResponse = "";
    public string textHistory = "";
    public event Action<string> OnNewPlayerResponse;
    public event Action<string> OnUpdateTextHistory;

    public void HandleSTTComplete(string latestPlayerResponse)
    {
        this.latestPlayerResponse = latestPlayerResponse;
        textHistory += "Player: " + latestPlayerResponse + "\n";

        OnNewPlayerResponse?.Invoke(latestPlayerResponse);
        OnUpdateTextHistory?.Invoke("Player: " + latestPlayerResponse + "\n");
        
        Debug.Log("LATEST PLAYER RESPONSE: " + this.latestPlayerResponse);
    }

    public void HandleStartTTS()
    {
        textHistory += "Avatar: " + latestAvatarResponse + "\n";

        OnUpdateTextHistory?.Invoke("Avatar: " + latestAvatarResponse + "\n");

        Debug.Log("LATEST AVATAR RESPONSE: " + latestAvatarResponse);
    }
}
