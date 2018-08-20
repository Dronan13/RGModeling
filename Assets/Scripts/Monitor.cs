using UnityEngine;
using UnityEngine.SceneManagement;

class Monitor : MonoBehaviour
{
    public GameObject[] robots;
    public GameObject grid;
    
    void Start()
    {
        robots = GameObject.FindGameObjectsWithTag("robot");

        System.IO.Directory.CreateDirectory(config.path + config.mode + "\\common" + "\\maps\\");

        System.IO.Directory.CreateDirectory(config.path + config.mode + "\\R1" + "\\maps\\");
        System.IO.Directory.CreateDirectory(config.path + config.mode + "\\R2" + "\\maps\\");
        System.IO.Directory.CreateDirectory(config.path + config.mode + "\\R3" + "\\maps\\");

        if (config.mode.Equals("Group")|| config.mode.Equals("Preknown"))
        {
            InvokeRepeating("MainTargetReachedForGroup", 1.0f, 1f);
        }

        if (config.mode.Equals("Single"))
        {
            foreach (GameObject robot in robots)
            {
                robot.SetActive(false);
            }
            //Debug.Log("Setting active robot to "+ config.activeRobotID.ToString());
            robots[config.activeRobotID].SetActive(true);
            
            InvokeRepeating("MainTargetReachedForOne", 1.0f, 1f);
        }

        if (config.mode.Equals("Preknown"))
        {
            
        }


    }

    void MainTargetReachedForGroup()
    {
        int reload = 0;

        foreach (GameObject robot in robots)
        {
            if (robot.GetComponent<Unit>().reached) reload++;
        }

        if (reload == robots.Length && config.experiment <= config.experiments)
        {           
            string array = "";
            string array_1 = "";
            string array_2 = "";
            string array_3 = "";

            for (int i = 0; i < grid.GetComponent<Grid>().grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetComponent<Grid>().grid.GetLength(1); j++)
                {
                    array += (grid.GetComponent<Grid>().grid[i, j].walkable ? "1" : "0") + " ";
                    array_1 += (grid.GetComponent<Grid>().grid_1[i, j].walkable ? "1" : "0") + " ";
                    array_2 += (grid.GetComponent<Grid>().grid_2[i, j].walkable ? "1" : "0") + " ";
                    array_3 += (grid.GetComponent<Grid>().grid_3[i, j].walkable ? "1" : "0") + " ";
                }
                array += System.Environment.NewLine;

                array_1 += System.Environment.NewLine;
                array_2 += System.Environment.NewLine;
                array_3 += System.Environment.NewLine;
            }

            string filename = "M" + config.experiment + ".txt";

            System.IO.File.WriteAllText(config.path + config.mode + "\\common" + "\\maps\\" + filename, array);
            System.IO.File.WriteAllText(config.path + config.mode + "\\R1" + "\\maps\\" + filename, array_1);
            System.IO.File.WriteAllText(config.path + config.mode + "\\R2" + "\\maps\\" + filename, array_2);
            System.IO.File.WriteAllText(config.path + config.mode + "\\R3" + "\\maps\\" + filename, array_3);


            config.experiment++;                    
            SceneManager.LoadScene(config.scene);
        }
        if (config.experiment>config.experiments)
        {
            SceneManager.LoadScene("Menu");
        } 
    }

    void MainTargetReachedForOne()
    {
        foreach (GameObject robot in robots)
        {
            if (robot.activeInHierarchy)
            {
                if (robot.GetComponent<Unit>().reached && config.experiment <= config.experiments)
                {
                    string array = "";

                    for (int i = 0; i < grid.GetComponent<Grid>().grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < grid.GetComponent<Grid>().grid.GetLength(1); j++)
                        {
                            array += (grid.GetComponent<Grid>().grid[i, j].walkable ? "1" : "0") + " ";
                        }
                        array += System.Environment.NewLine;
                    }

                    string filename = "M" + config.experiment + ".txt";

                    System.IO.File.WriteAllText(config.path + config.mode + "\\R"+ (config.activeRobotID +1).ToString() + "\\maps\\" + filename, array);

                    config.experiment++;                   
                    SceneManager.LoadScene(config.scene);
                }

                
                if (config.experiment > config.experiments)
                {
                    config.experiment = 1;
                    config.activeRobotID++;

                    if (config.activeRobotID > 2)
                    {
                        SceneManager.LoadScene("Menu");
                    }
                }

                
            }   
        }
        
    }

}

