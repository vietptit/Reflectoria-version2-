using System;
using System.Collections.Generic;
using UnityEngine;

public enum CheckAxis { X, Y, Z }

[Serializable]
class ConditionDetail
{
    public Transform objects;
    public List<Vector3> ros = new List<Vector3>();

    [Header("Cài đặt giới hạn vị trí")]
    public CheckAxis axisToCheck = CheckAxis.X; 
    public float minValue;
    public float maxValue; 
}

[Serializable]
class Condition
{
    public List<ConditionDetail> conditionDetails = new List<ConditionDetail>();
    public List<Waypoint> waypointsFollowCondition = new List<Waypoint>();

    public bool check()
    {
        foreach (var hit in conditionDetails)
        {
            bool isRotationValid = false;
            if (hit.ros == null || hit.ros.Count == 0) 
            {
                isRotationValid = true;
            }
            else
            {
                foreach (var check in hit.ros)
                {
                    float rotationDifference = Quaternion.Angle(hit.objects.transform.localRotation, Quaternion.Euler(check));
                    if (rotationDifference <= 15f)
                    {
                        isRotationValid = true;
                        break; 
                    }
                }
            }

            Vector3 currentPos = hit.objects.localPosition;
            float valueToCheck = 0f;

            switch (hit.axisToCheck)
            {
                case CheckAxis.X: valueToCheck = currentPos.x; break;
                case CheckAxis.Y: valueToCheck = currentPos.y; break;
                case CheckAxis.Z: valueToCheck = currentPos.z; break;
            }

            bool isOutOfBound = valueToCheck < hit.minValue || valueToCheck > hit.maxValue;

            
            if (!isRotationValid || isOutOfBound)
            {
                SetWayPointsActive(false);
                return false;
            }
        }

        SetWayPointsActive(true);
        return true;
    }

    void SetWayPointsActive(bool b)
    {
        foreach (var hit in waypointsFollowCondition)
        {
            hit.isActive = b;
        }
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    public Waypoint waypointTarget;
    public Waypoint waypointCurrent => Player.instance.GetCurrentWayPoint();
    public static event Action<List<Waypoint>> LetGoPlayer;
    
    List<Waypoint> path = new List<Waypoint>();
    [SerializeField] List<Condition> conditions = new List<Condition>();
    [SerializeField] List<Waypoint> destination = new List<Waypoint>();
    
    int idx = 0; 
    
    
    

    void Awake()
    {
        if(instance == null)  instance = this;
    }
    
    void Update()
    {
        if (idx >= destination.Count) return;

        foreach (var cond in conditions)
        {
            cond.check();
        }

        if (Player.instance.isMoving) return;
        
        waypointTarget = destination[idx];
        List<Waypoint> resultPath = PathFinding();
        
        if (resultPath != null && resultPath.Count > 0)
        {
            Player.instance.isMoving = true; 
            LetGoPlayer?.Invoke(resultPath); 
        }
    }

    public void OnPlayerReachedDestination()
    {
        idx++; 
        
        if (idx >= destination.Count)
        {
            IAmWinThenTriggerEvent();
        }
    }

    protected virtual void IAmWinThenTriggerEvent()
    {
        UFO_Controll.instance.Active();
        Debug.Log("Win");
        Main_Menu.instance.SaveLevel(); 
    }

    public List<Waypoint> PathFinding()
    {
        if (waypointCurrent == null) 
            Debug.LogError("1: player không biết nó đang đứng ở đâu.");
        
        if (waypointTarget == null) 
            Debug.LogError("2: Chưa có đích đến (Target is null). kiểm tra lại list Destination.");

        if (waypointCurrent != null && waypointTarget != null && waypointTarget != waypointCurrent)
        {
            Queue<Waypoint> waypointsQueue = new Queue<Waypoint>();
            HashSet<Waypoint> visited = new HashSet<Waypoint>(); 
            Dictionary<Waypoint, Waypoint> parentMap = new Dictionary<Waypoint, Waypoint>();

            waypointsQueue.Enqueue(waypointCurrent);
            visited.Add(waypointCurrent);

            bool found = false;
            Waypoint currentCheck = null;

            while (waypointsQueue.Count > 0)
            {
                currentCheck = waypointsQueue.Dequeue();
                
                if (currentCheck == waypointTarget)
                {
                    found = true;
                    break; 
                }

                List<Waypoint> listWayPointCurrent = currentCheck.GetListWayPont();
                for (int i = 0; i < listWayPointCurrent.Count; i++)
                {
                    Waypoint neighbor = listWayPointCurrent[i];                  
                    if (!visited.Contains(neighbor) && neighbor.isActive)
                    {
                        visited.Add(neighbor); 
                        parentMap[neighbor] = currentCheck; 
                        waypointsQueue.Enqueue(neighbor);  
                    }
                }
            }      
            
            if (found)
            {
                path.Clear(); 
                Waypoint step = waypointTarget;
                
                while (step != waypointCurrent)
                {
                    path.Add(step);
                    step = parentMap[step]; 
                }
                
                path.Reverse(); 
                return path;
            }

        }
        
        return null;
    }
}