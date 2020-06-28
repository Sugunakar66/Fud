﻿using frame8.ScrollRectItemsAdapter.GridExample;
using System.Collections.Generic;
using UnityEngine;


public class PortfolioOfferedView : MonoBehaviour
{
    public RectTransform content;

    public GameObject noDataObject;

    public ETabType tabType;

    public PortfolioActivityPopUp activityPopUp;

    public PortfolioOfferedTableView tableView;

    [HideInInspector]
    public List<PortfolioActivityModel> activityModels;


    bool isPagingOver = false;

    int pageNo = 1;

    int MAX_OFFERED_PORTFOLIOS = 50;


    public void Load()
    {
        LoadOfferedData();

        gameObject.SetActive(true);
    }

    void LoadOfferedData()
    {
        tableView.gameObject.SetActive(false);

        GameManager.Instance.apiHandler.GetPortfolioPosts(pageNo, (status, response) => {

            PortfolioPostResponse responseModel = JsonUtility.FromJson<PortfolioPostResponse>(response);

            Debug.Log("LoadOfferedData : Response = " + response);

            if (status)
            {
                activityModels = responseModel.data;

                pageNo++;

                if (activityModels.Count < MAX_OFFERED_PORTFOLIOS)
                {
                    isPagingOver = true;

                    pageNo = 1;
                }

                noDataObject.SetActive(activityModels.Count == 0);

                tableView.gameObject.SetActive(true);
            }
        });
    }

    public void OnAPICall()
    {
        if (isPagingOver)
            return;

        GetNextPageData();
    }

    void GetNextPageData()
    {
        GameManager.Instance.apiHandler.GetPortfolioPosts(pageNo, (status, response) =>
        {
            PortfolioPostResponse responseModel = JsonUtility.FromJson<PortfolioPostResponse>(response);

            if (status)
            {
                this.activityModels = responseModel.data;

                pageNo++;

                if (activityModels.Count < MAX_OFFERED_PORTFOLIOS)
                {
                    isPagingOver = true;

                    pageNo = 0;
                }
                else
                {
                    isPagingOver = false;

                    pageNo++;
                }

                tableView.Data.Clear();

                tableView.Data.Add(activityModels.Count);

                tableView.Refresh();
            }
        });
    }
}
