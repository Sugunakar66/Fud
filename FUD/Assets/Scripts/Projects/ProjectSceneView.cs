﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ProjectSceneView : MonoBehaviour
{
    public SceneCharacterView charactersView;

    public TMP_Dropdown shootTimeDropdown;
    public TMP_Dropdown placeDropdown;
    public TMP_InputField sceneOrderText;
    public TMP_InputField locationText;
    public TMP_InputField descriptionText;
    public TMP_Text startDateText;


    ProjectScenesPanel scenesPanel;

    SceneModel sceneModel;

    SceneDetailsModel detailsModel;

    List<Dictionary<string, object>> dialoguesList = new List<Dictionary<string, object>>();


    public void Load(SceneModel sceneModel, ProjectScenesPanel scenesPanel)
    {
        this.sceneModel = sceneModel;
        
        this.scenesPanel = scenesPanel;

        gameObject.SetActive(true);

        GameManager.Instance.apiHandler.GetSceneDetails(sceneModel.id, (status, response) => {

            if (status)
            {
                SceneResponse responseModel = JsonUtility.FromJson<SceneResponse>(response);

                detailsModel = responseModel.data;

                SetView();
            }
        });
    }

    void SetView()
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);

        DateTime selectedDate = dateTime.AddSeconds(detailsModel.start_time);

        sceneOrderText.text = detailsModel.scene_order.ToString();

        locationText.text = detailsModel.location;

        descriptionText.text = detailsModel.description;

        startDateText.text = DatePicker.Instance.GetDateString(selectedDate);

        shootTimeDropdown.captionText.text = detailsModel.shoot_time;

        placeDropdown.captionText.text = detailsModel.place_type;
    }

    public void BackButtonAction()
    {
        ResetData();

        gameObject.SetActive(false);
    }

    public void OnNextButtonAction()
    {
        charactersView.Load(detailsModel.SceneCharacters, detailsModel.ScenesMultimedia);
    }

    void ResetData()
    {
        sceneOrderText.text = locationText.text = descriptionText.text = startDateText.text = string.Empty;

        shootTimeDropdown.captionText.text = placeDropdown.captionText.text = string.Empty;
    }
}