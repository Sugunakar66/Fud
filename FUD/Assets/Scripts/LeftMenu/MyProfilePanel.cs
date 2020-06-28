﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MyProfilePanel : MonoBehaviour
{
    public Image profileIcon;

    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text emailText;
    public TMP_Text contactText;
    public TMP_Text roleText;
    public TMP_Text primarySkillsText;
    public TMP_Text secondarySkillsText;


    public ProfileInfoView infoView;


    public void SetView()
    {
        UserData data = DataManager.Instance.userInfo;

        List<Genre> genres = DataManager.Instance.genres;

        Genre selectedGenre = genres.Find(item => item.id == data.role_id);

        if (data != null)
        {
            nameText.text = data.name;
            ageText.text = data.dob;
            emailText.text = data.email_id;
            contactText.text = data.phone.ToString();
            roleText.text = selectedGenre.name;
            primarySkillsText.text = "";
            secondarySkillsText.text = "";
        }
        gameObject.SetActive(true);
    }
    public void OnBackButtonAction()
    {
        gameObject.SetActive(false);
    }

    public void OnEditButtonAction()
    {
        infoView.Load(OnCloseAction);
    }

    void OnCloseAction(bool isDataUpdated)
    {
        if (isDataUpdated)
        {
            SetView();
        }
    }
}
