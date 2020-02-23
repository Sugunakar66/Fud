﻿using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.IO;
using TMPro;


public class StoryCreationView : MonoBehaviour
{
    #region Singleton

    private static StoryCreationView instance = null;
    private StoryCreationView()
    {

    }

    public static StoryCreationView Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoryCreationView>();
            }
            return instance;
        }
    }

    #endregion
    public Transform parentPanel;

    public RectTransform galleryPanel;

    public TMP_InputField storyTitleField;

    public TMP_InputField subTitleField;

    public TMP_Dropdown dropdown;

    public TMP_InputField descriptionField;

    public Image screenShotImage;

    public TextMeshProUGUI contentType;

    public TextMeshProUGUI filePath;

    public TextMeshProUGUI statusText;

    public TextMeshProUGUI canSupportMultipleText;


    MyStoriesController storiesController;

    List<Genre> genres;

    List<string> imageUrls;

    System.Action OnClose;

    public void Load(System.Action onClose)
    {
        parentPanel.gameObject.SetActive(true);

        OnClose = onClose;

        canSupportMultipleText.text = "Can Support Text " + NativeGallery.CanSelectMultipleFilesFromGallery().ToString();

        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        genres = DataManager.Instance.genres;

        List<string> options = new List<string>();

        foreach (var option in genres)
        {
            options.Add(option.name);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    public void OnBackButtonAction()
    {
        parentPanel.gameObject.SetActive(false);
        OnClose?.Invoke();
        OnClose = null;
    }

    public void OnUploadAction()
    {
        ShowGalleryPanel();
    }

    public void OnSubmitAction()
    {
        string selectedGenreText = dropdown.options[dropdown.value].text;

        Genre selectedGenre = genres.Find(genre => genre.name.Equals(selectedGenreText));

        Debug.Log("Genre Id = " + selectedGenre.id);

        GameManager.Instance.apiHandler.CreateStory(storyTitleField.text, subTitleField.text, descriptionField.text, selectedGenre.id, (status, response) => {

            if (status)
            {
                Reset();

                OnBackButtonAction();
                Debug.Log("Story Uploaded Successfully");
            }
            else {
                Debug.LogError("Story Updation Failed");
            }
        });
    }

    public void OnScreenShotAction()
    {
        GetScreenShot();
    }

    public void OnVideosAction()
    {
        GetGalleryVideos();
    }

    public void OnCancelAction()
    { 
        
    }

    public void OnMediaButtonAction(int mediaType)
    {
        EMediaType selectedType = (EMediaType)mediaType;

        SlideGalleryView(false);

        switch (selectedType)
        {
            case EMediaType.Image:
                GalleryManager.Instance.PickImages(OnImagesUploaded);
                break;
            case EMediaType.Audio:
                GalleryManager.Instance.GetAudiosFromGallery();
                break;
            case EMediaType.Video:
                GalleryManager.Instance.GetVideosFromGallery();
                break;
        }
    }

    void Reset()
    {
        //storyTitleField.text = string.Empty;
        gameObject.SetActive(false);

        storyTitleField.text = string.Empty;

        subTitleField.text = string.Empty;

        descriptionField.text = string.Empty;
    }

    void OnImagesUploaded(bool status, List<string> imageUrls)
    {
        if (status)
        {
            this.imageUrls = imageUrls;
        }
    }

    void ShowGalleryPanel()
    {
        SlideGalleryView(true);
    }

    void SlideGalleryView(bool canShow)
    {
        float panelPosition = galleryPanel.anchoredPosition.y;

        float targetPostion = panelPosition += canShow ? galleryPanel.rect.height : -galleryPanel.rect.height;

        galleryPanel.DOAnchorPosY(targetPostion, 0.4f);
    }

    void GetScreenShot()
    { 
    
    }

    void GetGalleryVideos()
    { 
    
    }
}
