using UnityEngine;

public class WaypointGizmo : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private bool drawGizmos;

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
