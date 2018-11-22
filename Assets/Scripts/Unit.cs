using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    public int id;
    public bool reached { get; set; }
    const float minPathUpdateTime = .2f;
	const float pathUpdateMoveThreshold = .5f;
    public Transform[] targets;
    int targetID = 0;
    Transform target;

    float speed;

	Path path;

    List<Vector3> playerPos = new List<Vector3>();
    LineRenderer lineRenderer;

    void Start() {
        
        System.IO.Directory.CreateDirectory(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\traks\\");

        
        string trackHead = "time,x,y,z,roll,pitch,yaw"+System.Environment.NewLine;

        System.IO.File.AppendAllText(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\traks\\"
                                            + "\\TE" + config.experiment.ToString() + ".txt", trackHead);
        

        Color red = Color.red;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = red;
        lineRenderer.endColor = red;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;

        speed = config.speedLow;
        target = targets[targetID];

        InvokeRepeating("RequestPath", 2.0f, 0.3f);
    }

	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {

        if (pathSuccessful) {
			path = new Path(waypoints, transform.position, config.turnDst, config.stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");           
        }
        
	}

    void Update()
    {
        playerPos.Add(transform.position);
        lineRenderer.numPositions = playerPos.Count;
        for (int i = 0; i < playerPos.Count; i++)
        {
            lineRenderer.SetPosition(i, playerPos[i]);
        }

        string track = Time.realtimeSinceStartup.ToString() + ","
                     + transform.position.x.ToString() + ","
                     + transform.position.y.ToString() + ","
                     + transform.position.z.ToString() + ","
                     + transform.rotation.x.ToString() + ","
                     + transform.rotation.y.ToString() + ","
                     + transform.rotation.z.ToString()
                     + System.Environment.NewLine;

        System.IO.File.AppendAllText(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\traks\\"
                                            + "\\TE" + config.experiment.ToString() + ".txt", track);
    }
    public void RequestPath()
    {
        if (Vector3.Distance(transform.position, target.position) < 5 && targetID < (targets.Length - 1))
        {
            targetID++;
            target = targets[targetID];
        }

        if (Vector3.Distance(transform.position, target.position) < 5 && targetID == (targets.Length - 1) )
        {
            reached = true;
        }

        GameObject.Find("Grid").GetComponent<PathRequestManager>()
            .RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
    }

	IEnumerator FollowPath() {

		bool followingPath = true;
		int pathIndex = 0;

		float speedPercent = 1;

        float obstProx = GetComponent<TVSRay>().obstacleProximity;
        float maxVis = config.viewRadius;

        
       
        if (config.speedControll)
        {
            if (obstProx > 2 * maxVis / 3)
                speed = config.speedMax;

            if ((obstProx > (maxVis / 3)) & (obstProx <= (2 * maxVis / 3)))
                speed = config.speedMed;

            if (obstProx > 0 & obstProx <= (maxVis / 3))
                speed = config.speedLow;
        }
                      
        while (followingPath) {
			Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
			while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {

				if (pathIndex >= path.slowDownIndex && config.stoppingDst > 0) {
					speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / config.stoppingDst);
					if (speedPercent < 0.01f) {
						followingPath = false;
					}
				}

				Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints [pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * config.turnSpeed);
				transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
			}

			yield return null;

		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
            path.DrawWithGizmos(transform.position);
        }       
    }


}
