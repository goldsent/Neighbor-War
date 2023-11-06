using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  [SerializeField] private bool isTurn = false;
  [SerializeField] public bool IsTurn => isTurn;
  [SerializeField] private float hp;
  private float maxHp;
  private float amountDamage;
  private float criticalDamage;
  public float AmountDamage => amountDamage;
  public float CriticalDamage => criticalDamage;
  [SerializeField] private HealthBar healthBar;

  // setup player data
  public void StartInit(float healthPoint, float damage, float criticalDamage, float maxHealthPoint)
  {
    this.hp = healthPoint;
    this.maxHp = maxHealthPoint;
    this.amountDamage = damage;
    this.criticalDamage = criticalDamage;
    this.healthBar.StartInit(hp, maxHp);
  }

  // set player turn
  public void SetPlayerTurn(bool isturn)
  {
    this.isTurn = isturn;
  }
}