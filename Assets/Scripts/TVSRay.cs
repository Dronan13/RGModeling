using System;
using System.Collections.Generic;
using UnityEngine;

class TVSRay : MonoBehaviour
{
    public LayerMask obstacleMask;
    public float vertDispAngle = 90;

    float angle_ver;

    int stepCount;
    float stepAngleSize;
    //float angle;
    float angle2;

    public GameObject commonGrid;

    
    string scan;
    string frames = "";
    public float obstacleProximity { get; set; }

    void Awake()
    {
        stepCount = Mathf.RoundToInt(config.viewAngle * config.meshResolution);
        stepAngleSize = config.viewAngle / stepCount;
    }
    void Start()
    {

        System.IO.Directory.CreateDirectory(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\proximities\\");
        System.IO.Directory.CreateDirectory(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\frames\\");
        //frames = "{"+ System.Environment.NewLine+"\t\"frames\":["+ System.Environment.NewLine;      
        //frames = "[";//+ System.Environment.NewLine;                                
    }

    void OnApplicationQuit()
    {
        //frames = frames.Remove(frames.Length - 3) + System.Environment.NewLine;
        //frames += "\t]"+ System.Environment.NewLine; 

        System.IO.File.AppendAllText(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\frames\\"
                                            + "\\FE" + config.experiment.ToString() + ".json", frames);
    }
    void LateUpdate()
    {
        DrawFieldOfView();
        DrawFieldOfViewBeam();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(transform.rotation.y + globalAngle, true);
        RaycastHit hit;
        obstacleProximity = Vector3.Distance(transform.position, transform.position + dir * config.viewRadius);

        if (Physics.Raycast(transform.position, dir, out hit, config.viewRadius, obstacleMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.blue);
            Vector3 wp = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            commonGrid.GetComponent<Grid>().
                                        UpdateGrid(wp, GetComponent<Unit>().id, false);
            obstacleProximity = Vector3.Distance(transform.position, wp);
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + dir * config.viewRadius, Color.blue);
            return new ViewCastInfo(false, transform.position + dir * config.viewRadius, config.viewRadius, globalAngle);
        }
    }

    ViewCastInfo ViewCastBeam(float globalAngle)
    {
        Vector3 dir = DirFromAngleBeam(transform.rotation.y + globalAngle, true);
        RaycastHit hit;
        obstacleProximity = Vector3.Distance(transform.position, transform.position + dir * config.viewRadius);

        if (Physics.Raycast(transform.position, dir, out hit, config.viewRadius, obstacleMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Vector3 wp = new Vector3(hit.point.x, hit.point.y, hit.point.z);          
            obstacleProximity = Vector3.Distance(transform.position, wp);
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + dir * config.viewRadius, Color.red);
            return new ViewCastInfo(false, transform.position + dir * config.viewRadius, config.viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }        
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public Vector3 DirFromAngleBeam(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
                               Mathf.Cos(angle_ver * Mathf.Deg2Rad),
                               Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(config.viewAngle * config.meshResolution);
        float stepAngleSize = config.viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - config.viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }
    }
    
    void DrawFieldOfViewBeam()
    {
        int stepCount = Mathf.RoundToInt(config.viewAngle * config.meshResolution);
        int stepCount_ver = Mathf.RoundToInt(config.vertAngle * config.meshResolution);

        float stepAngleSize = config.viewAngle / stepCount;
        float stepAngleSize_ver = config.vertAngle / stepCount_ver;
        List<Vector3> viewPoints = new List<Vector3>();
        //string proximities = "";
        string frame = "";
                
        frame +="\t{" ;//+ System.Environment.NewLine;
        frame += "\t\"time\":"+ "\"" + DateTime.Now.ToString("hh.mm.ss.ffffff").ToString()+ "\",";// + System.Environment.NewLine;
        frame += "\t\"position\":[" 
                    + transform.position.x.ToString("G4") + ","
                    + transform.position.y.ToString("G4") + ","
                    + transform.position.z.ToString("G4")
                    + "],";
                    //+ System.Environment.NewLine;                   
        frame += "\t\"roation\":[" 
                    + transform.rotation.x.ToString("G4")+","
                    + transform.rotation.y.ToString("G4") + ","
                    + transform.rotation.z.ToString("G4")
                    + "]," ;
                    //+ System.Environment.NewLine;                    
        frame +="\t\"points\":[";//+ System.Environment.NewLine;

        for (int i = 0; i <= stepCount; i++)
        {          
            for (int j = 0; j <= stepCount_ver; j++)
            {
                float angle = transform.eulerAngles.y - config.viewAngle / 2 + stepAngleSize * i;
                angle_ver = -transform.eulerAngles.x + vertDispAngle - config.vertAngle / 2 + stepAngleSize_ver * j;               
                ViewCastInfo newViewCast = ViewCastBeam(angle);
                viewPoints.Add(newViewCast.point);
                if(obstacleProximity < (config.viewRadius-0.1)){
                   frame += "\t\t[" 
                    + newViewCast.point.x.ToString("G4") + ","
                    + newViewCast.point.y.ToString("G4") + ","
                    + newViewCast.point.z.ToString("G4") + ","
                    + obstacleProximity.ToString("G4")
                    + "],";// + System.Environment.NewLine; 
                }
                
            }       
        }
        //frame = frame.Remove(frame.Length - 3) + System.Environment.NewLine;
        frame += "\t\t]";// + System.Environment.NewLine;
        frame += "\t}" + System.Environment.NewLine;

        frames += frame;
        /* 
        proximities = proximities.Remove(proximities.Length - 1) + System.Environment.NewLine;
        Store proximities in seperate file
        System.IO.File.AppendAllText(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString() 
                                            + "\\proximities\\" 
                                            + "\\PE" + config.experiment.ToString() + ".txt", proximities);
        
        System.IO.File.AppendAllText(config.path
                                            + config.mode
                                            + "\\R" + GetComponent<Unit>().id.ToString()
                                            + "\\frames\\"
                                            + "\\FE" + config.experiment.ToString() + ".txt", frame);
        */
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}

