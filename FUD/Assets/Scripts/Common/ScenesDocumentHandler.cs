﻿using System.Collections.Generic;
using UnityEngine;
using System;


public class ScenesDocumentHandler : MonoBehaviour
{
    public RectTransform content;

    public GameObject cellCache;

    public RectTransform mediaButtonTrans;

    public bool isSingleImage = false;

    EMediaType mediaType;


    public void Load(List<MultimediaModel> mediaList, EMediaType mediaType = EMediaType.Image, Action<object> OnDeleteAction = null)
    {
        //content.DestroyChildrens();

        this.mediaType = mediaType;

        UploadedFileCell fileCell = null;

        for (int i = 0; i < mediaList.Count; i++)
        {
            GameObject cellObject = Instantiate(cellCache, content);

            fileCell = cellObject.GetComponent<UploadedFileCell>();

            fileCell.Load(mediaList[i], mediaType, OnDeleteAction);
        }

        if (mediaButtonTrans != null)
        {
            mediaButtonTrans.gameObject.SetActive(!isSingleImage);

            mediaButtonTrans.SetAsLastSibling();
        }
    }

    public void ClearData()
    {
        if (mediaButtonTrans != null)
        {
            content.DestroyChildrens(mediaButtonTrans.gameObject);
        }
        else
        {
            content.DestroyChildrens();
        }
    }
}