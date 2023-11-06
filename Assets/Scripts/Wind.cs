using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wind : MonoBehaviour
{
  [SerializeField] private Image arrowL, arrowR, barFillL, barFillR;
  private float directionWind;
  [SerializeField] private float windForce;
  [SerializeField] private float randomForce;

  [SerializeField] private int mode;

  public float calculateWind()
  {
    randomWind();
    windForce = randomForce * directionWind / 20f * 0.2f;
    updateWindBar(mode);
    return windForce;
  }

  // random wind.
  private void randomWind()
  {
    randomForce = Random.Range(0, 20);
    if (Mathf.RoundToInt(Random.Range(0, 10)) > 5)
    {
      mode = 1;
    }
    else
    {
      mode = 0;
    }
    if (mode == 0)
    {
      triggleArrow(true);
      directionWind = -1;
    }
    else
    {
      triggleArrow(false);
      directionWind = 1;
    }
  }

  //set arrow for wind direction.
  private void triggleArrow(bool isLeft)
  {
    arrowL.gameObject.SetActive(isLeft);
    arrowR.gameObject.SetActive(!isLeft);
  }

  //set Wind bar ui.
  private void updateWindBar(int mode)
  {
    if (mode == 0)
    {
      barFillL.fillAmount = -windForce / 0.2f;
      barFillR.fillAmount = 0;
    }
    else
    {
      barFillL.fillAmount = 0;
      barFillR.fillAmount = windForce / 0.2f;
    }
  }
}