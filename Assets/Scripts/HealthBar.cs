using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  [SerializeField] private Image healthbar;
  [SerializeField] private float hp;
  private GameControl gameControl;
  public float Hp => hp;
  private float maxHp;

  //setup Health bar
  public void StartInit(float healthPoint, float maxHealthPoint)
  {
    hp = healthPoint;
    maxHp = maxHealthPoint;
    updateHealthBar();
    gameControl = FindObjectOfType<GameControl>();
  }

  private void updateHealthBar()
  {
    healthbar.fillAmount = hp / maxHp;
  }

  //calcurate damgae and update healthbar
  public void calCulateDamage(float amount, Action<float> callback)
  {
    hp -= amount;
    updateHealthBar();
    if (hp <= 0)
    {
      gameControl.OnGameEnd(this);
    }
    callback?.Invoke(hp);
  }

  //Heal player
  public void addHp(float amountHp)
  {
    hp += amountHp;
    updateHealthBar();
  }

  //when hp over max hp ,hp is maxhp
  public void overHeal()
  {
    hp = maxHp;
  }
}