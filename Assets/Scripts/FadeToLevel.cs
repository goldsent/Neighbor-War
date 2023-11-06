using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToLevel : MonoBehaviour
{
  [SerializeField] private Animator animator;
  [SerializeField] private GameObject UserProfile;
  private int indexLevelToLoad;

  public void FadeOutToLevel(int index)
  {
    animator.SetTrigger("IsTrigger");
    indexLevelToLoad = index;
    Invoke("CompleteFadeOut", 1f);
  }

  private void CompleteFadeOut()
  {
    DontDestroyOnLoad(UserProfile);
    SceneManager.LoadScene(indexLevelToLoad);
    RectTransform rect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
    UserProfile.GetComponent<RectTransform>().SetAsLastSibling();
  }
}