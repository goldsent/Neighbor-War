using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public enum PlayerSide
{
  PigRich,
  AuntNextDoor,
  AiEnemy
}

public enum GameMode
{
  VersusAi,
  VersusPlayer
}

public class GameControl : MonoBehaviour
{
  public GameMode GameCurrentMode = GameMode.VersusPlayer;
  [SerializeField] private Player pig;
  [SerializeField] private Player aunt;
  [SerializeField] private AiEnemy aiEnemy;
  [SerializeField] private float pigHp;
  [SerializeField] private float auntHp;
  [SerializeField] private float pigMaxHp;
  [SerializeField] private float auntMaxHp;
  [SerializeField] private float pigAmountDamage;
  [SerializeField] private float auntAmountDamage;
  [SerializeField] private float pigCriticalDamage;
  [SerializeField] private float auntCriticalDamage;
  [SerializeField] private AnimationCharactor pigAnimation;
  [SerializeField] private AnimationCharactor auntAnimation;
  [SerializeField] private HealthBar pigHealthBar;
  [SerializeField] private HealthBar auntHealthBar;

  [SerializeField] private Button pigHeal;
  [SerializeField] private Button auntHeal;
  [SerializeField] private Button pigDoubleAttack;
  [SerializeField] private Button auntDoubleAttack;
  [SerializeField] private Button pigHeavyAttack;
  [SerializeField] private Button auntHeavyAttack;

  [SerializeField] private Wind windSystem;
  [SerializeField] private GameObject userProfile;
  [SerializeField] private GameObject scorllBoard;
  [SerializeField] private List<Image> extraItem;

  private int healAmount = 10;

  private int heavyDamageScale = 2;
  public int HeavyDamageScale => heavyDamageScale;
  private bool isHeavyAttack = false;
  public bool IsHeavyDamage => isHeavyAttack;
  private bool isDoubleAttack = false;
  public bool IsDoubleAttack => isDoubleAttack;

  private PlayerSide currentPlayerSide = PlayerSide.AuntNextDoor;
  public PlayerSide CurrentPlayerSide => currentPlayerSide;
  private float windForce;
  public float WindForce => windForce;

  [SerializeField] private TMP_Text winLossText;
  [SerializeField] private TMP_Text timeForPlay;
  private float startTime;

  private void Update()
  {
  }

  // Change turn.
  public void SwitchPlayerSide()
  {
    switch (GameCurrentMode)
    {
      case GameMode.VersusPlayer:
        // Current turn is Aunt.
        if (currentPlayerSide == PlayerSide.AuntNextDoor)
        {
          currentPlayerSide = PlayerSide.PigRich;
          toggleInteractBtn(true);
          pig.SetPlayerTurn(true);
          aunt.SetPlayerTurn(false);
          windForce = windSystem.calculateWind();
        }
        else
        {
          // Current turn is pig.
          currentPlayerSide = PlayerSide.AuntNextDoor;
          toggleInteractBtn(false);
          pig.SetPlayerTurn(false);
          aunt.SetPlayerTurn(true);
          windForce = windSystem.calculateWind();
        }
        break;

      case GameMode.VersusAi:
        Debug.Log("turn charge");
        if (currentPlayerSide == PlayerSide.PigRich)
        {
          // Current turn is AiEnemy.
          currentPlayerSide = PlayerSide.AuntNextDoor;
          toggleInteractBtn(false);
          aiEnemy.SetAiTurn(false);
          aunt.SetPlayerTurn(true);
          windForce = windSystem.calculateWind();
        }
        else
        {
          // Currenturn is player.
          currentPlayerSide = PlayerSide.PigRich;
          toggleInteractBtn(false);
          aiEnemy.SetAiTurn(true);
          aunt.SetPlayerTurn(false);
          windForce = windSystem.calculateWind();
        }
        break;
    }
  }

  // End game with hp < 0.
  public void OnGameEnd(HealthBar healthBar)
  {
    // check time for playing.
    timeForPlay.text = ((Time.time - startTime) / 60f).ToString("F") + " M";
    if (pigHealthBar.Hp == healthBar.Hp)
    {
      if (GameCurrentMode == GameMode.VersusAi)
      {
        // ai loss.
        aiEnemy.SetAiTurn(false);
      }
      else
      {
        // pig win.
        pig.SetPlayerTurn(false);
      }
      winLossText.text = "Aunt Win!!!";
      aunt.SetPlayerTurn(false);
      pigAnimation.LossAnimation();
      auntAnimation.WinAnimation();
      scorllBoard.SetActive(true);
    }
    if (auntHealthBar.Hp == healthBar.Hp)
    {
      if (GameCurrentMode == GameMode.VersusAi)
      {
        //ai win.
        aiEnemy.SetAiTurn(false);
      }
      else
      {
        //Aunt loss.
        pig.SetPlayerTurn(false);
      }
      winLossText.text = "Pig Win!!!";
      aunt.SetPlayerTurn(false);
      auntAnimation.LossAnimation();
      pigAnimation.WinAnimation();
      scorllBoard.SetActive(true);
    }
  }

  public void Heal(int healAmount)
  {
    if (currentPlayerSide == PlayerSide.AuntNextDoor && auntHealthBar.Hp < auntMaxHp)
    {
      setCloseInteractiveBtn();
      auntHealthBar.addHp(healAmount);
      auntHeal.gameObject.GetComponent<Image>().enabled = false;
      auntHeal.interactable = false;
      if (auntHealthBar.Hp > 100) auntHealthBar.overHeal();
    }
    if (currentPlayerSide == PlayerSide.PigRich && pigHealthBar.Hp < pigMaxHp)
    {
      setCloseInteractiveBtn();
      pigHealthBar.addHp(healAmount);
      pigHeal.gameObject.GetComponent<Image>().enabled = false;
      pigHeal.enabled = false;
      if (auntHealthBar.Hp > 100) pigHealthBar.overHeal();
    }
  }

  // reset interact extra item.
  public void setCloseInteractiveBtn()
  {
    pigHeal.interactable = false;
    pigDoubleAttack.interactable = false;
    pigHeavyAttack.interactable = false;

    auntHeal.interactable = false;
    auntDoubleAttack.interactable = false;
    auntHeavyAttack.interactable = false;
  }

  // togle interact extra item.
  private void toggleInteractBtn(bool isPigside)
  {
    if (GameCurrentMode == GameMode.VersusPlayer)
    {
      pigHeal.interactable = isPigside;
      pigDoubleAttack.interactable = isPigside;
      pigHeavyAttack.interactable = isPigside;

      auntHeal.interactable = !isPigside;
      auntDoubleAttack.interactable = !isPigside;
      auntHeavyAttack.interactable = !isPigside;
    }
    else if (GameCurrentMode == GameMode.VersusAi)
    {
      auntHeal.interactable = !isPigside;
      auntDoubleAttack.interactable = !isPigside;
      auntHeavyAttack.interactable = !isPigside;
    }
  }

  public void doubleAttack()
  {
    setCloseInteractiveBtn();
    isDoubleAttack = true;
    if (CurrentPlayerSide == PlayerSide.PigRich)
    {
      pigDoubleAttack.gameObject.GetComponent<Image>().enabled = false;
    }
    else
    {
      auntDoubleAttack.gameObject.GetComponent<Image>().enabled = false;
    }
  }

  public void resetDoubleAttack()
  {
    isDoubleAttack = false;
  }

  public void HeavyDamage()
  {
    setCloseInteractiveBtn();
    isHeavyAttack = true;
    if (CurrentPlayerSide == PlayerSide.PigRich)
    {
      pigHeavyAttack.gameObject.GetComponent<Image>().enabled = false;
    }
    else
    {
      auntHeavyAttack.gameObject.GetComponent<Image>().enabled = false;
    }
  }

  public void resetHeavyAttack()
  {
    isHeavyAttack = false;
  }

  private void addActionItemBtn()
  {
    pigHeal.onClick.AddListener(() =>
    {
      Heal(healAmount);
    });

    auntHeal.onClick.AddListener(() =>
    {
      Heal(healAmount);
    });

    pigDoubleAttack.onClick.AddListener(() =>
    {
      doubleAttack();
    });

    auntDoubleAttack.onClick.AddListener(() =>
    {
      doubleAttack();
    });

    pigHeavyAttack.onClick.AddListener(() =>
    {
      HeavyDamage();
    });

    auntHeavyAttack.onClick.AddListener(() =>
    {
      HeavyDamage();
    });
  }

  // Replay in same mode.
  public void ReplayGame()
  {
    switch (GameCurrentMode)
    {
      case GameMode.VersusPlayer:
        {
          setupGameplay();
          break;
        }

      case GameMode.VersusAi:
        {
          setupGameplayWithAi();
          break;
        }
    }
  }

  // set up extra item.
  private void setActiveItem()
  {
    foreach (Image item in extraItem)
    {
      item.enabled = true;
    }
  }

  // set up game play with player vs player.
  public void setupGameplay()
  {
    GameCurrentMode = GameMode.VersusPlayer;
    currentPlayerSide = PlayerSide.AuntNextDoor;
    startTime = Time.time;
    pig.StartInit(pigHp, pigAmountDamage, pigCriticalDamage, pigMaxHp);
    aunt.StartInit(auntHp, auntAmountDamage, auntCriticalDamage, auntMaxHp);
    //set aunt play first.
    aunt.SetPlayerTurn(true);
    setActiveItem();
    toggleInteractBtn(false);
    addActionItemBtn();
    pigAnimation.ResetAnimationToIdle();
    auntAnimation.ResetAnimationToIdle();
    windForce = windSystem.calculateWind();
  }

  //set login guess.
  public void SetupLogin()
  {
    userProfile.SetActive(true);
  }

  // set up game play with player vs Ai.
  public void setupGameplayWithAi()
  {
    GameCurrentMode = GameMode.VersusAi;
    currentPlayerSide = PlayerSide.AuntNextDoor;
    startTime = Time.time;
    aiEnemy.StartInit();
    aunt.StartInit(auntHp, auntAmountDamage, auntCriticalDamage, auntMaxHp);
    aunt.SetPlayerTurn(true);
    setActiveItem();
    toggleInteractBtn(false);
    addActionItemBtn();
    pigAnimation.ResetAnimationToIdle();
    auntAnimation.ResetAnimationToIdle();
    windForce = windSystem.calculateWind();
  }
}