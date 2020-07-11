﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class AuditionOfferedCell : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text ageText;
    public TMP_Text rateOfPayText;

    Audition auditionData;
    AuditionOfferedView auditionController;
    ETabType auditionType;


    List<Dictionary<string, object>> uploadedDict = new List<Dictionary<string, object>>();

    public void SetView(AuditionOfferedView auditionController, Audition audition)
    {
        auditionData = audition;
        this.auditionController = auditionController;

        auditionType = auditionController.tabType;

        if (auditionData != null)
        {
            titleText.text = auditionData.topic;

            ageText.text = "Age : " + auditionData.age_to.ToString();

            rateOfPayText.text = "Budget : " + auditionData.rate_of_pay;
        }
    }

    public void OnClickAction()
    {
        AuditionJoinView.Instance.Load(auditionData, false, (index) =>
        {
            switch (index)
            {
                case 8:
                case 3:
                case 5:
                    UpdateAuditionStatus(index);
                    break;

                case 9://View
                    auditionController.LoadAuditionDetails(auditionData);
                    break;

                case 10:
                    GalleryButtonsPanel.Instance.Load(MediaButtonAction);
                    break;

                case 11:
                    RecordVideo();
                    break;
            }
        });
    }

    void RecordVideo()
    {
        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            string fileName = Path.GetFileName(path);

            byte[] fileBytes = File.ReadAllBytes(path);

            titleText.text = fileBytes.Length.ToString();

            NativeGallery.SaveVideoToGallery(fileBytes, "Videos", fileName);

            GalleryManager.Instance.UploadVideoFile(path, OnVideoUploaded);
        });
    }

    void OnVideoUploaded(bool status, List<string> videoURL)
    {
        if (status)
        {
            for (int i = 0; i < videoURL.Count; i++)
            {
                Dictionary<string, object> kvp = new Dictionary<string, object>();

                kvp.Add("content_id", 1);

                kvp.Add("content_url", videoURL[i]);

                kvp.Add("media_type", "video");

                uploadedDict.Add(kvp);
            }
        }

        JoinAudition();
    }

    void Refresh()
    {
        auditionController?.GetAuditions();
    }

    void MediaButtonAction(int index)
    {
        EMediaType selectedType = (EMediaType)index;
        AlertMessage.Instance.SetText(index + "  " + selectedType);
        uploadedDict = new List<Dictionary<string, object>>();

        switch (selectedType)
        {
            case EMediaType.Image:
                GalleryManager.Instance.PickImages(OnImagesUploaded);
                break;
            case EMediaType.Video:
                GalleryManager.Instance.GetVideosFromGallery(OnVideosUploaded);
                break;
            case EMediaType.Audio:
                GalleryManager.Instance.GetAudiosFromGallery(OnAudiosUploaded);
                break;
        }
    }

    void OnImagesUploaded(bool status, List<string> imageUrls)
    {
        AlertMessage.Instance.SetText("OnImagesUploaded/" + status);
        if (status)
        {
            for (int i = 0; i < imageUrls.Count; i++)
            {
                Dictionary<string, object> kvp = new Dictionary<string, object>();

                kvp.Add("content_id", 1);

                kvp.Add("content_url", imageUrls[i]);

                kvp.Add("media_type", "image");

                uploadedDict.Add(kvp);
            }
            JoinAudition();
        }
    }

    void OnAudiosUploaded(bool status, List<string> audioUrls)
    {
        if (status)
        {
            for (int i = 0; i < audioUrls.Count; i++)
            {
                Dictionary<string, object> kvp = new Dictionary<string, object>();

                kvp.Add("content_id", 1);

                kvp.Add("content_url", audioUrls[i]);

                kvp.Add("media_type", "audio");

                uploadedDict.Add(kvp);
            }
            JoinAudition();
        }
    }

    void OnVideosUploaded(bool status, List<string> videoUrls)
    {
        if (status)
        {
            for (int i = 0; i < videoUrls.Count; i++)
            {
                Dictionary<string, object> kvp = new Dictionary<string, object>();

                kvp.Add("content_id", 1);

                kvp.Add("content_url", videoUrls[i]);

                kvp.Add("media_type", "video");

                uploadedDict.Add(kvp);
            }
            JoinAudition();
        }
    }

    void JoinAudition()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("audition_id", auditionData.id);
        parameters.Add("port_album_media", uploadedDict);
        GameManager.Instance.apiHandler.JoinAudition(parameters, (status, response) =>
        {
            if (status)
            {
                AlertModel alertModel = new AlertModel();
                alertModel.message = "Audition Joined Successfully";
                alertModel.okayButtonAction = Refresh;
                alertModel.canEnableTick = true;
                UIManager.Instance.ShowAlert(alertModel);

                DataManager.Instance.UpdateFeaturedData(EFeatureType.AuditionJoining);
            }
            else
            {
                AlertModel alertModel = new AlertModel();
                alertModel.message = "Joining Audition Failed";
                UIManager.Instance.ShowAlert(alertModel);
            }
        });
    }

    void UpdateAudition()
    {
        AlertMessage.Instance.SetText("AuditionCell/UpdateAudition");
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        //parameters.Add("audition_id", auditionData.id);
        parameters.Add("id", auditionData.project_id);
        //parameters.Add("user_id", DataManager.Instance.userInfo.id);
        parameters.Add("pro_multi_media", uploadedDict);

        GameManager.Instance.apiHandler.UpdateJoinedAudition(parameters, (status, response) =>
        {

            AlertMessage.Instance.SetText(status + "==" + response);

            if (status)
            {
                AlertModel alertModel = new AlertModel();
                alertModel.message = "Audition Updated Successfully";
                alertModel.okayButtonAction = Refresh;
                alertModel.canEnableTick = true;
                UIManager.Instance.ShowAlert(alertModel);
            }
            else
            {
                AlertModel alertModel = new AlertModel();
                alertModel.message = "Updating Audition Failed";
                UIManager.Instance.ShowAlert(alertModel);
            }
        });
    }

    void UpdateAuditionStatus(int statusIndex)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();

        parameters.Add("id", auditionData.project_id);

        parameters.Add("status", statusIndex);

        GameManager.Instance.apiHandler.UpdateJoinedAudition(parameters, (status, response) =>
        {
            if (status)
            {
                AlertModel alertModel = new AlertModel();
                alertModel.message = "Audition Status updation success";
                alertModel.okayButtonAction = Refresh;
                alertModel.canEnableTick = true;
                UIManager.Instance.ShowAlert(alertModel);
            }
            else
            {
                AlertModel alertModel = new AlertModel();
                alertModel.message = "Audition Status updation failed";
                UIManager.Instance.ShowAlert(alertModel);
            }
        });
    }

}