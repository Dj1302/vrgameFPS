using Unity.Netcode;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void HostGame()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started!");
        }
    }

    public void JoinGame()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client joined!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
