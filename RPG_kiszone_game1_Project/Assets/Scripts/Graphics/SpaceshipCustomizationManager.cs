using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceshipCustomizationManager : MonoBehaviour
{
    public List<Material> startMaterials;
    public List<SpaceshipGeneratorPreset> startSpaceships;
    //[SerializeField] MeshRenderer spaceshipMeshRenderer;
    [SerializeField] SpaceshipGenerator playerSpaceshipGenerator;
    Transform platform;
    float hue = 0f;
    float saturation = 0f;
    float value = 1f;
    private void Awake()
    {
        platform = GameObject.Find("SpaceShipPlatform").transform;
        if (GameData.availableSpaceships == null)
        {
            for (int j = 0; j < 10; j++)
            {
                SpaceshipGeneratorPreset sgp = ScriptableObject.CreateInstance<SpaceshipGeneratorPreset>();
                sgp.CopyPreset(startSpaceships[0]);
                sgp.name = sgp.name.Substring(0, 7) + j + ": ";
                for (int i = 0; i < sgp.passes.Count; i++)
                {
                    Pass tmp_pass = sgp.passes[i];
                    int seedincr =  Random.Range(0, 100);
                    tmp_pass.seed = seedincr;
                    sgp.name += seedincr + " ";
                    sgp.passes[i] = tmp_pass;
                }
                startSpaceships.Add(sgp);
            }
            startSpaceships.RemoveAt(0);
            GameData.availableSpaceships = startSpaceships;
        }
            
        if (GameData.availableMaterials == null)
            GameData.availableMaterials = startMaterials;
        SetMaterial(GameData.selectedMaterialId);
        SetMaterialDropdown(GameData.selectedMaterialId);
        SetSliders();

        
        SetShapePicker();
    }

    void SetSliders()
    {
        Color.RGBToHSV(GameData.availableMaterials[0].color, out hue, out saturation, out value);
        GameObject.Find("HueSlider").GetComponent<Slider>().value = hue;
        GameObject.Find("SaturationSlider").GetComponent<Slider>().value = saturation;
        GameObject.Find("ValueSlider").GetComponent<Slider>().value = value;
    }

    void SetMaterialDropdown(int matId = 0)
    {
        TMP_Dropdown materialDropdown = GameObject.Find("MaterialDropdown").GetComponent<TMP_Dropdown>();
        materialDropdown.options.Clear();
        foreach(Material mat in GameData.availableMaterials)
            materialDropdown.options.Add(new TMP_Dropdown.OptionData(mat.name));
        materialDropdown.value = matId;
    }

    void SetShapePicker()
    {
        TMP_Text spaceshipName = GameObject.Find("SpaceshipModelName").GetComponent<TMP_Text>();
        spaceshipName.text = GameData.availableSpaceships[GameData.selectedSpaceshipId].name;
        playerSpaceshipGenerator.SetPreset(GameData.availableSpaceships[GameData.selectedSpaceshipId]);
    }

    public void SetMaterial(int matId = 0)
    {
        GameData.selectedMaterialId = matId;
        foreach (MeshRenderer mr in playerSpaceshipGenerator.GetMeshRenderers())
        {
            mr.material = GameData.availableMaterials[GameData.selectedMaterialId];
        }
    }

    void ChangeColor()
    {
        foreach (Material mat in GameData.availableMaterials)
        {
            mat.color = Color.HSVToRGB(hue, saturation, value);
        }
    }

    public void ChangeHue(float h)
    {
        hue = h;
        ChangeColor();
    }
    public void ChangeSaturation(float sat)
    {
        saturation = sat;
        ChangeColor();
    }
    public void ChangeValue(float val)
    {
        value = val;
        ChangeColor();
    }

    public void ChangeShape(int increment)
    {
        GameData.selectedSpaceshipId += increment;
        while (GameData.selectedSpaceshipId < 0) GameData.selectedSpaceshipId += GameData.availableSpaceships.Count;
        if (GameData.selectedSpaceshipId >= GameData.availableSpaceships.Count) GameData.selectedSpaceshipId -= GameData.availableSpaceships.Count;

        SetShapePicker();
    }

    [SerializeField] SecondOrderDynamics spaceshipRotation;
    private void Start()
    {
        spaceshipRotation.Initialise();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E)) platform.Rotate(Vector3.up * Time.deltaTime * spaceshipRotation.Update(0f));
        else if (Input.GetKey(KeyCode.Q)) platform.Rotate(Vector3.up * Time.deltaTime * spaceshipRotation.Update(50f));
        else if (Input.GetKey(KeyCode.E)) platform.Rotate(Vector3.up * Time.deltaTime * spaceshipRotation.Update(-50f));
        else platform.Rotate(Vector3.up * Time.deltaTime * spaceshipRotation.Update(0f));
    }
}
