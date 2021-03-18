using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab.SharedModels;
using UnityEngine;

public class PF_Advertising
{
    public static void GetAdPlacements(GetAdPlacementsRequest request, Action<GetAdPlacementsResult> resultCallback, Action<PlayFab.PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
    {
        PlayFabClientAPI.GetAdPlacements(request, resultCallback, errorCallback, extraHeaders);
    }

    public static void ReportAdActivity(string placementId, string rewardId, AdActivity activity)
    {
        PlayFabClientAPI.ReportAdActivity(new ReportAdActivityRequest()
        {
            PlacementId = placementId,
            RewardId = rewardId,
            Activity = AdActivity.End
        },
        (result) =>
        {
            if (activity == AdActivity.End)
                RewardAdActivity(placementId, rewardId);
        },
        (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public static void RewardAdActivity(string placementId, string rewardId)
    {
        PlayFabClientAPI.RewardAdActivity(new RewardAdActivityRequest()
        {
            PlacementId = placementId,
            RewardId = rewardId,
        },
         (result) =>
         {
             Debug.Log("GrantedVirtualCurrencies:" + result.RewardResults.GrantedVirtualCurrencies["MS"]);             
         },
         (error) =>
         {
             Debug.Log(error.GenerateErrorReport());
         });
    }
}
