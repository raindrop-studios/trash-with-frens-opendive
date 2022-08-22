using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SystemSetting : MonoBehaviour
{
    public static SystemSetting instance { get; set; }

    public bool isLocalTest;

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
