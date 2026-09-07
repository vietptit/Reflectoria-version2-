using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] List<Waypoint> waypoints = new List<Waypoint>();
    [SerializeField] LayerMask layerMask;

    public bool isActive = false;
    public bool noRotate = false;

    [Header("Cài đặt Đặc biệt")]
    [Tooltip("Tích vào đây nếu cục này nằm trong GameManager (Condition). Nó sẽ KHÔNG bị xóa khỏi list và KHÔNG tự động active khi đến gần.")]
    public bool isSpecialCondition = false;
    [Tooltip("Tích vào đây nếu cục này nằm trong gương. Nó sẽ KHÔNG bị xóa khỏi list và KHÔNG tự động active khi đến gần.")]
    public bool inMirror = false;

    [Header("Cài đặt Quét Hàng Xóm")]
    [Tooltip("Độ dài của tia quét chữ thập")]
    public float scanRadius = 0.8f; 
    
    [Tooltip("Khoảng cách tối đa giữa 2 tâm. Nếu khối trượt chưa tới đủ gần sẽ không nhận hàng xóm (Thường là 1.05f cho lưới 1x1)")]
    public float maxNeighborDistance = 1.05f;

    void Update()
    {
        DynamicCheckWaypoints();
    }

    public Vector3 GetWalkPosition()
    {
        Vector3 trueCenter = transform.position - (transform.up * 0.5f);
        return trueCenter + (Vector3.up * 0.5f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.green : Color.red;
        if (isSpecialCondition) Gizmos.color = Color.yellow;
        
        Gizmos.DrawSphere(GetWalkPosition(), 0.1f);

        
        Gizmos.color = Color.cyan; 
        Vector3 blockCenter = transform.position - (transform.up * 0.5f);
        
        Vector3[] crossDirections = new Vector3[]
        {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right
        };

        foreach (Vector3 dir in crossDirections)
        {
            Gizmos.DrawLine(blockCenter, blockCenter + dir * scanRadius);
            Gizmos.DrawWireCube(blockCenter + dir * scanRadius, Vector3.one * 0.2f);
        }

        
        if (waypoints != null && waypoints.Count > 0)
        {
            Gizmos.color = Color.magenta; 
            
            foreach (Waypoint neighbor in waypoints)
            {
                if (neighbor != null)
                {
                    Gizmos.DrawLine(GetWalkPosition(), neighbor.GetWalkPosition());
                    Vector3 midPoint = (GetWalkPosition() + neighbor.GetWalkPosition()) / 2f;
                    Gizmos.DrawWireCube(midPoint, Vector3.one * 0.15f);
                }
            }
        }
    }

    void DynamicCheckWaypoints()
    {
        List<Waypoint> currentNeighbors = new List<Waypoint>();

       
        Vector3 blockCenter = transform.position - (transform.up * 0.5f);

        
        Vector3[] crossDirections = new Vector3[]
        {
            Vector3.forward, 
            Vector3.back,    
            Vector3.left,    
            Vector3.right   
        };

        
        foreach (Vector3 dir in crossDirections)
        {
            RaycastHit[] hits = Physics.BoxCastAll(blockCenter, Vector3.one * 0.2f, dir, Quaternion.identity, scanRadius, layerMask);

            foreach (RaycastHit hit in hits)
            {
                Waypoint wp = hit.collider.GetComponent<Waypoint>();
                
                
                if (wp != null && wp != this && !currentNeighbors.Contains(wp))
                {
                    
                    float dist = Vector3.Distance(this.GetWalkPosition(), wp.GetWalkPosition());

                    if (dist <= maxNeighborDistance)
                    {
                        currentNeighbors.Add(wp);
                    }
                }
            }
        }

        // ==========================================
        // QUẢN LÝ LIST WAYPOINTS 
        // ==========================================
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            Waypoint wp = waypoints[i];
            if (wp.isSpecialCondition) continue;

            if (!currentNeighbors.Contains(wp))
            {
                wp.isActive = false;
                waypoints.RemoveAt(i);
            }
        }

        foreach (var neighbor in currentNeighbors)
        {
            if (!waypoints.Contains(neighbor))
            {
                waypoints.Add(neighbor);
            }

            if (!neighbor.isSpecialCondition && !neighbor.inMirror)
            {
                neighbor.isActive = true;
            }
        }
    }

    public List<Waypoint> GetListWayPont() => waypoints;

    void OnTriggerEnter(Collider other)
    {
        if (!inMirror) return;

        if (other.gameObject.tag == "Mirror")
        {
            isActive = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!inMirror) return;

        if (other.gameObject.tag == "Mirror")
        {
            isActive = false;
        }
    }
}