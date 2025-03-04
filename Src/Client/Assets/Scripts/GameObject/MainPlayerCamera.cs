using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public new Camera camera;
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
        if(camera == null)
        {
            player = User.Instance.CurrentCharacterObject;
        }
        if(player == null)
        {
            return;
        }

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
