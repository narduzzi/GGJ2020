using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkManager
{
    public static IEnumerator SendRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.0.193:8080/get_next_question?current_id=0&question_id=0&response_id=0");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
}
