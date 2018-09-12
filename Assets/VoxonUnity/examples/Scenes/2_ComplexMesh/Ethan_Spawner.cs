
using System.Collections.Generic;
using System;
using UnityEngine;

public class Ethan_Spawner : MonoBehaviour {

    public GameObject spawnable;
    public int max_ethans;
    List<GameObject> ethans;
    
	// Use this for initialization
	void Start () {
        ethans = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.frameCount % 360 == 0 && ethans.Count < max_ethans)
        {
            try
            {
                ethans.Add(Instantiate(spawnable, new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)), Quaternion.identity));
                ethans[ethans.Count - 1].AddComponent<Voxon.VXGameObject>();
            }
            catch (Exception E)
            {
                Debug.LogError("Error while Ethaning " + gameObject.name + ": " + E.Message);
            }
        }

        if (Time.frameCount % 360 == 0 && ethans.Count >= max_ethans)
        {
            GameObject fatal_ethan = ethans[0];
            ethans.RemoveAt(0);
            Destroy(fatal_ethan);
        }

    }
}
