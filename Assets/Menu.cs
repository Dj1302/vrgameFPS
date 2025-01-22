using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void HostGame()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started!");
            SceneManager.LoadScene("BasicScene", LoadSceneMode.Single);
            
        }
    }

    public void JoinGame()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            SceneManager.LoadScene("BasicScene", LoadSceneMode.Single);
            Debug.Log("Client joined!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
