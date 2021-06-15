using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyBuldozering;

    private BuildingPreset curBuildingPreset;

    private float indicatorUpdateRate = 0.05f;
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;

    public GameObject placementIndicator;
    public GameObject bulldozerIndicator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelBuildingPlacement();

        if (Time.time - lastUpdateTime > indicatorUpdateRate)
        {
            lastUpdateTime = Time.time;
            curIndicatorPos = Selector.instance.GetCurTilePosition();

            if (currentlyPlacing)
                placementIndicator.transform.position = curIndicatorPos;
            else if (currentlyBuldozering)
                bulldozerIndicator.transform.position = curIndicatorPos;
        }

        if (Input.GetMouseButtonDown(0) && currentlyPlacing)
            PlaceBuilding();
        else if (Input.GetMouseButtonDown(0) && currentlyBuldozering)
            Bulldoze();
    }

    public void BeginNewBuildingPlacement(BuildingPreset preset)
    {
        //check money
        currentlyPlacing = true;
        curBuildingPreset = preset;
        placementIndicator.SetActive(true);
        placementIndicator.transform.position = new Vector3(0, -99, 0);
    }

    void CancelBuildingPlacement()
    {
        currentlyPlacing = false;
        placementIndicator.SetActive(false);

        currentlyBuldozering = false;
        bulldozerIndicator.SetActive(false);
    }

    public void ToggleBulldoze()
    {
        currentlyBuldozering = !currentlyBuldozering;
        bulldozerIndicator.SetActive(currentlyBuldozering);
        bulldozerIndicator.transform.position = new Vector3(0, -99, 0);

    }

    void PlaceBuilding()
    {
        GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curIndicatorPos, Quaternion.identity);

        //tell city script
        City.instance.OnPlaceBuilding(buildingObj.GetComponent<Building>());

        CancelBuildingPlacement();
    }

    void Bulldoze()
    {
        Building buildingToDestroy = City.instance.buildings.Find(x => x.transform.position == curIndicatorPos);

        if (buildingToDestroy != null)
        {
            City.instance.OnRemoveBuilding(buildingToDestroy);
        }
    }
}
