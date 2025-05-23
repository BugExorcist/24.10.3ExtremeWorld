using UnityEngine;
using UnityEngine.AI;

public class NavPathRenderer : MonoSingleton<NavPathRenderer>
{
    private LineRenderer lineRenderer;
    private NavMeshPath path;
    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    internal void SetPath(NavMeshPath path, Vector3 target)
    {
        this.path = path;
        if (this.path == null)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
        }
        else
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = path.corners.Length + 1;
            lineRenderer.SetPositions(path.corners);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, target);
            for(int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}
