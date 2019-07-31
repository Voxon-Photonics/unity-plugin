using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
	public string LevelName;

	[Tooltip("Seconds")]
	public float LoadTime;
    // Start is called before the first frame update
    void Start()
    {
		Invoke("DelayedStart",LoadTime);
	}

	void DelayedStart()
	{
		StartCoroutine(LoadScene());
	}

	IEnumerator LoadScene()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LevelName);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
	// Update is called once per frame
	void Update()
    {
        
    }

	
}
