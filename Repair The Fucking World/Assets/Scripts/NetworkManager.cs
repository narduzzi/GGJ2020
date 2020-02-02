using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkManager
{
    public static IEnumerator NextQuestion(int NextEventID)
    {
        string request = string.Format(
            "http://35.238.231.226:8080/get_event?id={0}",
            NextEventID);

        UnityWebRequest www = UnityWebRequest.Get(request);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AnimationController.CurrentEvent = Event.LoadFromJSON(www.downloadHandler.text);
        }
    }
}
