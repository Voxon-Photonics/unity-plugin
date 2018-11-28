
using System.Collections.Generic;
using System;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour {

    public GameObject spawnable;
    public int max_chars;
    List<GameObject> chars;
    
	// Use this for initialization
	void Start () {
        chars = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(chars.Count == 0 || (Time.frameCount % 173 == 0 && chars.Count < max_chars))
        {
            try
            {
                chars.Add(Instantiate(spawnable, new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)), Quaternion.identity));
                chars[chars.Count - 1].AddComponent<Voxon.VXGameObject>();
            }
            catch (Exception E)
            {
                Debug.LogError("Error while Ethaning " + gameObject.name + ": " + E.Message);
            }
        }

        if (Time.frameCount % 180 == 0 && chars.Count >= max_chars)
        {
            GameObject fatal_ethan = chars[0];
            chars.RemoveAt(0);
            Destroy(fatal_ethan);
        }

    }
}
