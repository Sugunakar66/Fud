﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class CreateCharacterView : MonoBehaviour
{
    public RectTransform searchContent;

    public TMP_InputField castField;

    public TextMeshProUGUI genderLabel;

    public TMP_InputField suitableField;

    public TMP_InputField descriptionField;

    public GameObject searchCell;

    public GameObject scrollObject;


    StoryDetailsModel detailsModel;

    UserSearchModel selectedModel = null;

    string keyword = string.Empty;

    bool isSearchAPICalled = false;

    string apiResponse = string.Empty;

    Action<StoryCharacterModel> OnCreateCharacter;


    public void SetView(StoryDetailsModel detailsModel, Action<StoryCharacterModel> action)
    {
        this.detailsModel = detailsModel;

        OnCreateCharacter = action;
    }

    void PopulateDropdown(List<UserSearchModel> searchModels)
    {
        searchContent.DestroyChildrens();

        GameObject cellObject = null;

        if (searchModels.Count > 0)
        {
            scrollObject.SetActive(true);

            for (int i = 0; i < searchModels.Count; i++)
            {
                cellObject = Instantiate(searchCell, searchContent);

                cellObject.GetComponent<UserSearchCell>().SetView(searchModels[i], OnSelectMember);
            }
        }
    }

    public void OnValueChange()
    {
        if (selectedModel == null)
        {
            if (suitableField.text.Length > 2 && !isSearchAPICalled)
            {
                //Call Search API
                isSearchAPICalled = true;

                keyword = suitableField.text;

                GetSearchedUsers();
            }
        }
    }

    public void OnButtonAction()
    {
        if (!CanCallAPI())
        {
            return;
        }

        GameManager.Instance.apiHandler.CreateCharacter(detailsModel.id, castField.text, descriptionField.text, genderLabel.text, selectedModel.id, (status, response) => {

            if (status)
            {
                apiResponse = response;
            }

            OnAPIResponse(status);
        });
    }

    void OnAPIResponse(bool status)
    {
        AlertModel alertModel = new AlertModel();

        alertModel.message = status ? "Story Character Creation Success" : "Something went wrong, please try again.";

        if (status)
        {
            alertModel.okayButtonAction = OnSuccessResponse;

            alertModel.canEnableTick = true;
        }

        UIManager.Instance.ShowAlert(alertModel);
    }

    void OnSuccessResponse()
    {
        UpdatedCharaterModel responseModel = JsonUtility.FromJson<UpdatedCharaterModel>(apiResponse);

        StoryCharacterModel characterModel = responseModel.data;

        OnCreateCharacter?.Invoke(characterModel);

        Destroy(gameObject);

        apiResponse = string.Empty;
    }

    bool CanCallAPI()
    {
        string errorMessage = string.Empty;

        if (string.IsNullOrEmpty(castField.text))
        {
            errorMessage = "Character name should not be empty";
        }
        else if (string.IsNullOrEmpty(suitableField.text))
        {
            errorMessage = "Suitable performer should not be empty";
        }
        else if (string.IsNullOrEmpty(descriptionField.text))
        {
            errorMessage = "Story description should not be empty";
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            AlertModel alertModel = new AlertModel();
            alertModel.message = errorMessage;
            UIManager.Instance.ShowAlert(alertModel);
            return false;
        }

        return true;
    }

    void OnSelectMember(object _selectedModel)
    {
        this.selectedModel = _selectedModel as UserSearchModel;

        suitableField.text = selectedModel.name;

        searchContent.DestroyChildrens();

        scrollObject.SetActive(false);
    }

    void GetSearchedUsers()
    {
        GameManager.Instance.apiHandler.SearchTeamMember(keyword, (status, response) =>
        {
            if (status)
            {
                UserSearchResponse searchResponse = JsonUtility.FromJson<UserSearchResponse>(response);

                PopulateDropdown(searchResponse.data);

                isSearchAPICalled = false;
            }
        });
    }
}
