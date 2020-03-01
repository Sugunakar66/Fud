﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class CreateTeamView : MonoBehaviour
{
    public RectTransform searchContent;

    public TMP_InputField teamNameField;

    public TMP_InputField memberField;

    public GameObject searchCell;


    StoryDetailsModel detailsModel;

    UserSearchModel selectedModel;

    List<UserSearchModel> addedModels = new List<UserSearchModel>();

    Action<StoryTeamModel> OnAddedTeam; 

    bool isSearchAPICalled = false;

    string keyword = string.Empty;

    string inputData = string.Empty;



    public void SetView(StoryDetailsModel detailsModel, Action<StoryTeamModel> OnAddedTeam)
    {
        this.detailsModel = detailsModel;

        this.OnAddedTeam = OnAddedTeam;
    }

    void PopulateDropdown(List<UserSearchModel> searchModels)
    {
        searchContent.DestroyChildrens();

        GameObject cellObject = null;

        for (int i = 0; i < searchModels.Count; i++)
        {
            cellObject = Instantiate(searchCell, searchContent);

            cellObject.GetComponent<UserSearchCell>().SetView(searchModels[i], OnSelectMember);
        }
    }

    public void OnValueChange()
    {
        string addedMember = string.Empty;

        if (addedModels.Count > 0)
        {
            addedMember = memberField.text.Split(',').Last();
        }
        else {
            addedMember = memberField.text;
        }

        Debug.Log("addedMember = " + addedMember);

        if (addedMember.Length > 2 && !isSearchAPICalled)
        {
            isSearchAPICalled = true;

            keyword = addedMember;

            GetSearchedUsers();
        }
    }

    public void OnButtonAction()
    {
        string[] membersList = memberField.text.Split(',');

        List<string> member = new List<string>(membersList);

        string members = GetMemberIds(member);

        GameManager.Instance.apiHandler.UpdateStoryTeam(detailsModel.id, detailsModel.title, members, (status, response) =>
        {
            if (status)
            {
                UpdatedTeamModel responseModel = JsonUtility.FromJson<UpdatedTeamModel>(response);

                StoryTeamModel teamModel = responseModel.data;

                OnAddedTeam?.Invoke(teamModel);

                Destroy(gameObject);
            }
        });
    }

    void OnSelectMember(object _selectedModel)
    {
        Debug.Log("OnSelectMember Called");

        this.selectedModel = _selectedModel as UserSearchModel;

        if (!inputData.Contains(selectedModel.name))
        {
            addedModels.Add(selectedModel);

            memberField.text = string.Empty;

            memberField.text += inputData + selectedModel.name + ",";

            inputData = memberField.text;

            keyword = string.Empty;

            searchContent.DestroyChildrens();
        }
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

    string GetMemberIds(List<string> members)
    {
        string memberIds = string.Empty;

        for (int i = 0; i < members.Count; i++)
        {
            UserSearchModel addModel = addedModels.Find(searchModel => searchModel.name.Equals(members[i]));

            if (addModel != null)
            {
                memberIds += addModel.id + ",";
            }
        }

        return memberIds;
    }
}