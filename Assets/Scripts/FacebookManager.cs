using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Facebook.Unity;
using System;

public class FacebookManager : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI FB_userName;

  //public Image FB_profilePic;
  [SerializeField] private Image FB_profilePic;

  [SerializeField] private GameControl gameControl;

  #region Initialize

  private void Awake()
  {
    FB.Init(SetInit, onHidenUnity);

    if (!FB.IsInitialized)
    {
      FB.Init(() =>
      {
        if (FB.IsInitialized)
          FB.ActivateApp();
        else
          print("Couldn't initialize");
      },
      isGameShown =>
      {
        if (!isGameShown)
          Time.timeScale = 0;
        else
          Time.timeScale = 1;
      });
    }
    else
      FB.ActivateApp();
  }

  private void SetInit()
  {
    if (FB.IsLoggedIn)
    {
      Debug.Log("Facebook is Login!");
      string s = "client token" + FB.ClientToken + "User Id" + AccessToken.CurrentAccessToken.UserId + "token string" + AccessToken.CurrentAccessToken.TokenString;
      DealWithFbMenus(FB.IsLoggedIn);
    }
    else
    {
      DealWithFbMenus(FB.IsLoggedIn);
      Debug.Log("Facebook is not Logged in!");
    }
  }

  private void onHidenUnity(bool isGameShown)
  {
    if (!isGameShown)
    {
      Time.timeScale = 0;
    }
    else
    {
      Time.timeScale = 1;
    }
  }

  private void DealWithFbMenus(bool isLoggedIn)
  {
    if (isLoggedIn)
    {
      FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
      FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
    }
    else
    {
      print("Not logged in");
    }
  }

  private void DisplayUsername(IResult result)
  {
    if (result.Error == null)
    {
      string name = "" + result.ResultDictionary["first_name"];
      if (FB_userName != null) FB_userName.text = name;
      FB_userName.text = name;
      Debug.Log("" + name);
    }
    else
    {
      Debug.Log(result.Error);
    }
  }

  private void DisplayProfilePic(IGraphResult result)
  {
    if (result.Texture != null)
    {
      Debug.Log("Profile Pic");
      if (FB_profilePic != null)
      {
        FB_profilePic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        Debug.Log("Profile Pic 2");
      }
      /*JSONObject json = new JSONObject(result.RawResult);

      StartCoroutine(DownloadTexture(json["picture"]["data"]["url"].str, profile_texture));*/
    }
    else
    {
      Debug.Log(result.Error);
    }
  }

  #endregion Initialize

  //login
  public void Facebook_LogIn()
  {
    List<string> permissions = new List<string>();
    permissions.Add("public_profile");
    //permissions.Add("user_friends");
    FB.LogInWithReadPermissions(permissions, AuthCallBack);
  }

  private void AuthCallBack(IResult result)
  {
    if (FB.IsLoggedIn)
    {
      gameControl.SetupLogin();
      SetInit();
      //AccessToken class will have session details
      var aToken = AccessToken.CurrentAccessToken;

      print(aToken.UserId);

      foreach (string perm in aToken.Permissions)
      {
        print(perm);
      }
    }
    else
    {
      print("Failed to log in");
    }
  }

  //logout
  public void Facebook_LogOut()
  {
    StartCoroutine(LogOut());
  }

  private IEnumerator LogOut()
  {
    FB.LogOut();
    while (FB.IsLoggedIn)
    {
      print("Logging Out");
      yield return null;
    }
    print("Logout Successful");
    // if (FB_profilePic != null) FB_profilePic.sprite = null;
    if (FB_userName != null) FB_userName.text = "";
    if (FB_profilePic != null) FB_profilePic.sprite = null;
  }

  #region other

  public void FacebookSharefeed()
  {
    string url = "https://www.facebook.com/sent.kj/";
    FB.ShareLink(
        new Uri(url),
        "Neighbor War",
        "Share Fun to Friend",
        null,
        ShareCallback);
  }

  private static void ShareCallback(IShareResult result)
  {
    Debug.Log("ShareCallback");
    SpentCoins(2, "sharelink");
    if (result.Error != null)
    {
      Debug.LogError(result.Error);
      return;
    }
    Debug.Log(result.RawResult);
  }

  public static void SpentCoins(int coins, string item)
  {
    var param = new Dictionary<string, object>();
    param[AppEventParameterName.ContentID] = item;
    FB.LogAppEvent(AppEventName.SpentCredits, (float)coins, param);
  }

  /*public void GetFriendsPlayingThisGame()
  {
      string query = "/me/friends";
      FB.API(query, HttpMethod.GET, result =>
      {
          Debug.Log("the raw" + result.RawResult);
          var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
          var friendsList = (List<object>)dictionary["data"];

          foreach (var dict in friendsList)
          {
              GameObject go = Instantiate(friendstxtprefab);
              go.GetComponent<Text>().text = ((Dictionary<string, object>)dict)["name"].ToString();
              go.transform.SetParent(GetFriendsPos.transform, false);
              FriendsText[1].text += ((Dictionary<string, object>)dict)["name"];
          }
      });
  }*/

  #endregion other
}