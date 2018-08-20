using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

    
    //GENERAL
    public InputField path;   
    public InputField experiments;

    public Dropdown scene;
    public Dropdown mode;

    //UNIT
    public Toggle speedControll;
    public Toggle fullNodeList;

    public InputField speedMax;
    public InputField speedMed;
    public InputField speedLow;

    public InputField turnSpeed;
    public InputField turnDst;

    //TVS
    public InputField viewRadius;
    public InputField viewAngle;
    public InputField vertAngle;
    public InputField meshResolution;

    public InputField nodeRadius;
    public void Start()
    {
        speedControll.isOn = config.speedControll;
        fullNodeList.isOn = config.fullNodeList;

        path.text = config.path.ToString();
        experiments.text = config.experiments.ToString();

        speedMax.text = config.speedMax.ToString();
        speedMed.text = config.speedMed.ToString();
        speedLow.text = config.speedLow.ToString();

        turnSpeed.text = config.turnSpeed.ToString();
        turnDst.text = config.turnDst.ToString();

        viewRadius.text = config.viewRadius.ToString();
        viewAngle.text = config.viewAngle.ToString();
        vertAngle.text = config.vertAngle.ToString();
        meshResolution.text = config.meshResolution.ToString();

        nodeRadius.text = config.nodeRadius.ToString();
    }

    public void SceneLoadButton()
    {
        config.scene = scene.options[scene.value].text;
        config.mode = mode.options[mode.value].text;

        config.speedControll = speedControll.isOn;
        config.fullNodeList = fullNodeList.isOn;

        config.path = path.text;

        int.TryParse(experiments.text, out config.experiments);

        float.TryParse(speedMax.text, out config.speedMax);
        float.TryParse(speedMed.text, out config.speedMed);
        float.TryParse(speedLow.text, out config.speedLow);

        float.TryParse(turnSpeed.text, out config.turnSpeed);
        float.TryParse(turnDst.text, out config.turnDst);

        float.TryParse(viewRadius.text, out config.viewRadius);
        float.TryParse(viewAngle.text, out config.viewAngle);
        float.TryParse(vertAngle.text, out config.vertAngle);
        float.TryParse(meshResolution.text, out config.meshResolution);

        float.TryParse(nodeRadius.text, out config.nodeRadius);

        SceneManager.LoadScene(config.scene);
    }

}
