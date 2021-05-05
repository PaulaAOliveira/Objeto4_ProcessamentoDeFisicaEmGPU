using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verificador : MonoBehaviour
{

    RandColorCS rd = new RandColorCS();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        rd.taNoChao();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
