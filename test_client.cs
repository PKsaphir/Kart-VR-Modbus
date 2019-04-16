using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModBus;

public class test_client : MonoBehaviour
{

	public Component jeep;
    // Start is called before the first frame update
    void Start()
    {
        jeep = gameObject.GetComponent<NetKartMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
