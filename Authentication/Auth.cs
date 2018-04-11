using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Auth {

	public static IEnumerator AsyncPost(string host, string dir, 
		Dictionary<string, string> postParams = null, Action<bool, string> callback = null) {
		string uri = host + dir;

		using (UnityWebRequest postRequest = UnityWebRequest.Post(uri, postParams)) {
			yield return postRequest.Send();

			if (callback != null) {
				if (postRequest.isError) {
					callback(false, null);
				} else {
					callback(true, postRequest.downloadHandler.text);
				}
			}
		}
	}

}
