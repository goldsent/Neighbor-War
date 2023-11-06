using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AiMode
{
  Easy,
  Normal,
  Hard
}

public class AiEnemy : MonoBehaviour
{
  public AiMode aiMode = AiMode.Easy;
  [SerializeField] private Player player; // ระบุผู้เล่นที่ AI ควบคุม
  private GameControl gameControl;
  private float healthPoint;
  private float criticalDamage;
  private float damage;
  private float maxHealthPoint;
  private float percentMissChange = 0.5f;
  private bool isTurn = false;
  private float BasethrowPower = 0.195f;
  public bool IsTurn => isTurn;
  private bool isMissAttack = false;
  private bool alreadyRandom = false;
  [SerializeField] private Button auntHeal;
  [SerializeField] private Button auntDoubleAttack;
  [SerializeField] private Button auntHeavyAttack;
  private Image auntHealImage;
  private Image auntDoubleAttackImage;
  private Image auntHeavyAttackImage;

  private void Start()
  {
    gameControl = FindAnyObjectByType<GameControl>();
    auntHealImage = auntHeal.GetComponent<Image>();
    auntDoubleAttackImage = auntDoubleAttack.GetComponent<Image>();
    auntHeavyAttackImage = auntHeavyAttack.GetComponent<Image>();
  }

  //setup ai mode.
  private void setupAiMode()
  {
    switch (aiMode)
    {
      case AiMode.Easy:
        healthPoint = 50;
        maxHealthPoint = 50;
        damage = 5;
        percentMissChange = 0.5f;
        criticalDamage = 8;
        break;

      case AiMode.Normal:
        healthPoint = 60;
        maxHealthPoint = 60;
        percentMissChange = 0.3f;
        damage = 5;
        criticalDamage = 8;
        break;

      case AiMode.Hard:
        healthPoint = 50;
        maxHealthPoint = 50;
        percentMissChange = 0.15f;
        damage = 5;
        criticalDamage = 8;
        break;
    }
  }

  // calulate wind for throwing.
  public float CalculateForceWithWind()
  {
    IsMissAttack(percentMissChange);
    if (isMissAttack)
    {
      BasethrowPower = Random.Range(0, 0.3f);
      return BasethrowPower;
    }
    else
    {
      BasethrowPower = 0.195f;
      BasethrowPower = BasethrowPower - (gameControl.WindForce * 0.3f);
      return BasethrowPower;
    }
  }

  // Random ai for use extra item.
  private void isUseItem()
  {
    float randomValue = Random.value;
    if (randomValue > 0.5f)
    {
      if (auntHeavyAttackImage.enabled != false)
      {
        gameControl.HeavyDamage();
      }
      else if (auntDoubleAttackImage.enabled != false)
      {
        gameControl.doubleAttack();
      }
      else if (auntHealImage.enabled != false)
      {
        gameControl.Heal(10);
      }
    }
  }

  public void SetAiTurn(bool isturn)
  {
    this.isTurn = isturn;
  }

  //setup ai.
  public void StartInit()
  {
    setupAiMode();
    player.StartInit(healthPoint, damage, criticalDamage, maxHealthPoint);
  }

  //random change to attack.
  private void IsMissAttack(float attackMissChance)
  {
    if (alreadyRandom == false)
    {
      isUseItem();
      float randomValue = Random.value; // สุ่มค่าระหว่าง 0 ถึง 1.
      Debug.Log(randomValue);
      isMissAttack = randomValue <= attackMissChance;
    }
    alreadyRandom = true;
  }

  public void ResetRandomAlreadyChange()
  {
    alreadyRandom = false;
  }

  //set stage for ai mode.
  public void SetEasyMode()
  {
    aiMode = AiMode.Easy;
  }

  public void SetNormalMode()
  {
    aiMode = AiMode.Normal;
  }

  public void SetHardMode()
  {
    aiMode = AiMode.Hard;
  }
}