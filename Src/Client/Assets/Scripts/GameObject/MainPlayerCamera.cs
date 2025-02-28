using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera camera;
    public Transform viewPoint;

    public GameObject player;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if(player == null)
        {
            return;
        }

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
