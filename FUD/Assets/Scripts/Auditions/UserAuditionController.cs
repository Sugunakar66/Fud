﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserAuditionController : MonoBehaviour
{

    public enum EAuditionStatusScreen
    { 
        None = -1,
        Live = 0,
        Shortlisted = 1,
    }

    public GameObject userAuditionCell;

    public Transform activeContent;

    public Transform shortListedContent;

    public GameObject buttonsPanel;

    public GameObject livePanel;

    public GameObject shortListedPanel;

    public TextMeshProUGUI[] buttonList;


    public Color selectedColor;

    public Color disabledColor;


    EAuditionStatusScreen currentType = EAuditionStatusScreen.None;

    List<SearchAudition> activeAuditions;

    List<SearchAudition> shortListedAudtions;

    SearchAudition selectedAudition;

    AuditionController auditionController;

    int auditionId;

    public void SetView(AuditionController auditionController, List<SearchAudition> auditions, int auditionId)
    {
        this.auditionController = auditionController;

        this.activeAuditions = auditions;

        this.auditionId = auditionId;

        gameObject.SetActive(true);

        OnTabAction(0);
    }

    public void OnTabAction(int tabIndex)
    {
        UpdateScreen(tabIndex);
    }

    void UpdateScreen(int index)
    {
        EAuditionStatusScreen screenSubType = (EAuditionStatusScreen)index;

        if (currentType != screenSubType)
        {
            if (currentType != EAuditionStatusScreen.None)
            {
                buttonList[(int)currentType].color = disabledColor;
            }

            //noDataObject.SetActive(false);

            currentType = screenSubType;

            buttonList[(int)currentType].color = selectedColor;

            UpdateScreen();
        }
    }

    void UpdateScreen()
    {
        shortListedPanel.SetActive(currentType == EAuditionStatusScreen.Shortlisted);

        livePanel.SetActive(currentType == EAuditionStatusScreen.Live);

        switch (currentType)
        {
            case EAuditionStatusScreen.Live:
                LoadLiveAuditions();
                break;
            case EAuditionStatusScreen.Shortlisted:
                LoadShortListedAuditions();
                break;
        }
    }

    void LoadLiveAuditions()
    {
        if (activeContent.childCount > 0)
            return;
        
        activeContent.DestroyChildrens();

        for (int i = 0; i < activeAuditions.Count; i++)
        {
            GameObject storyObject = Instantiate(userAuditionCell, activeContent);

            UserAuditionCell item = storyObject.GetComponent<UserAuditionCell>();

            item.SetView(activeAuditions[i], OnAuditionSelectAction);
        }
    }

    void LoadShortListedAuditions()
    {
        if (shortListedAudtions == null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("id", auditionId);
            parameters.Add("page", 0);
            parameters.Add("limit", 20);
            parameters.Add("status", "shortlisted");

            GameManager.Instance.apiHandler.SearchAuditions(parameters, (status, response) => {

                if (status)
                {
                    SearchAuditionResponse auditionResponse = JsonUtility.FromJson<SearchAuditionResponse>(response);

                    if (auditionResponse.data.Count > 0)
                    {
                        shortListedAudtions = auditionResponse.data;

                        UpdateShortListedView();
                    }
                }
            });
        }
    }

    void UpdateShortListedView()
    {
        for (int i = 0; i < shortListedAudtions.Count; i++)
        {
            GameObject storyObject = Instantiate(userAuditionCell, shortListedContent);

            UserAuditionCell item = storyObject.GetComponent<UserAuditionCell>();

            item.SetView(activeAuditions[i], OnAuditionSelectAction);
        }
    }

    void OnAuditionSelectAction(SearchAudition audition)
    {
        selectedAudition = audition;
        buttonsPanel.SetActive(true);
    }

    void Reload()
    {
        gameObject.SetActive(false);
        auditionController.GetAuditions();
    }

    public void AcceptButtonAction()
    {
        buttonsPanel.SetActive(false);
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("audition_id", selectedAudition.audition_id);
        parameters.Add("user_audition_id", selectedAudition.id);
        parameters.Add("status", "selected");
        GameManager.Instance.apiHandler.AcceptOrRejectAudition(parameters, (status, response) => {
            if (status)
            {
                Reload();
            }
        });
    }

    public void RejectButtonAction()
    {
        buttonsPanel.SetActive(false);
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("audition_id", selectedAudition.audition_id);
        parameters.Add("user_audition_id", selectedAudition.id);
        parameters.Add("status", "rejected");
        GameManager.Instance.apiHandler.AcceptOrRejectAudition(parameters, (status, response) => {
            if (status)
            {
                Reload();
            }
        });
    }

    public void OnCancelButtonAction()
    {
        buttonsPanel.SetActive(false);
    }

    public void OnBackButtonAction()
    {
        gameObject.SetActive(false);

        activeAuditions = null;

        shortListedAudtions = null;

        currentType = EAuditionStatusScreen.None;
    }
}