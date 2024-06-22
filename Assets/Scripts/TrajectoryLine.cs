using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    private LineRenderer lr;
    private Transform start;
    private Transform end;
    private GameObject cannon;
    [SerializeField] float length;
    [SerializeField] GameObject testStart;
    [SerializeField] GameObject testEnd;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        cannon = transform.parent.gameObject;
        start = testStart.transform;
        end = testEnd.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetPositions();
    }

    public void SetPositions()
    {
        UpdateTrajectoryPoint();
        lr.SetPosition(0, start.position);
        lr.SetPosition(1, end.position);
    }

    // Updates end point of the trajectory line based on horizontal rotation of the cannon
    // TODO: Use GetStartPosition() to detect the ground instead of hard-coding the height of each point
    public void UpdateTrajectoryPoint()
    {
        end.position = start.position + cannon.transform.forward * length;
        end.position = new Vector3(end.position.x, 1, end.position.z);
        start.position = new Vector3(start.position.x, 1, start.position.z);
    }

    // TODO: Uses raycast from point on the cannon to find where the ground is
    public void GetStartPosition()
    {

    }
}
