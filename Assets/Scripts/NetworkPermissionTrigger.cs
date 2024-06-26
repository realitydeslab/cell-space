using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPermissionTrigger : MonoBehaviour
{
    // URL to trigger network permission - it should be a valid URL
    private string testUrl = "https://www.baidu.com";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RequestNetworkPermission());
    }

    IEnumerator RequestNetworkPermission()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(testUrl))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log($"Error requesting network permission: {webRequest.error}");
            }
            else
            {
                Debug.Log("Network permission has been triggered successfully.");
            }
        }
    }
}
