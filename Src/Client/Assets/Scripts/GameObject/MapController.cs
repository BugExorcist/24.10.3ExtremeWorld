using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Collider minimapBourdingBox;
    void Start()
    {
        MiniMapManager.Instance.UpdateMiniMap(minimapBourdingBox);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
