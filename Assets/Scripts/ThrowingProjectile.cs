using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThrowingProjectileyer : MonoBehaviour
{
  [SerializeField] private Sprite HeavyBullet;
  [SerializeField] private GameObject throwObj;
  [SerializeField] private float throwPower = 0f;
  [SerializeField] private Image chargeThowForce;
  [SerializeField] private float throwForce = 100f;
  [SerializeField] private float throwAngle = 50f;
  [SerializeField][Range(1, 10)] private float pingpongSpeed = 10f;
  [SerializeField] private RectTransform chargeBar;
  private float pressHoldTime = 0f;
  [SerializeField] private Player player;
  private Collider2D col;
  private GameControl gameControl;
  private Vector2 throwDir;
  [SerializeField] private AiEnemy aiEnemy;

  private void Start()
  {
    col = GetComponent<Collider2D>();
    gameControl = FindObjectOfType<GameControl>();
  }

  private void Update()
  {
    if (Input.GetKey(KeyCode.Mouse0) && player.IsTurn && !isPointerOverUIObject())
    {
      calculateThrowForce();
      chargeBar.gameObject.SetActive(true);
    }

    if (Input.GetMouseButtonUp(0) && player.IsTurn && pressHoldTime != 0)
    {
      gameControl.setCloseInteractiveBtn();
      ThrowObject();
      chargeBar.gameObject.SetActive(false);
      Invoke("resetPara", 3f);
      player.SetPlayerTurn(false);
    }

    if (aiEnemy != null)
    {
      if (aiEnemy.IsTurn)
      {
        calculateThrowForceAi();
      }
    }
  }

  private void ThrowObject()
  {
    if (gameControl.CurrentPlayerSide == PlayerSide.PigRich)
    {
      //create bullet.
      GameObject BulletObj = Instantiate(throwObj, transform);
      if (gameControl.IsHeavyDamage)
      {
        // set damage to bullet.
        BulletObj.GetComponent<Bullet>().SetDamge(player.AmountDamage * gameControl.HeavyDamageScale, player.CriticalDamage * gameControl.HeavyDamageScale);
        // change image for heavy attack.
        BulletObj.GetComponent<SpriteRenderer>().sprite = HeavyBullet;
      }
      else
      {
        // set damage to bullet.
        BulletObj.GetComponent<Bullet>().SetDamge(player.AmountDamage, player.CriticalDamage);
      }
      BulletObj.GetComponent<Bullet>().GetAnimationCharactor(GetComponent<AnimationCharactor>());
      //set angle for throw.
      throwDir = Quaternion.Euler(0, 0, throwAngle) * Vector2.right;
      BulletObj.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce * throwPower / 0.03f);
      //add wind force.
      BulletObj.GetComponent<Rigidbody2D>().velocity = new Vector2(gameControl.WindForce * 20f, 0);
      gameControl.resetHeavyAttack();
      if (gameControl.IsDoubleAttack)
      {
        // attack 2 time.
        StartCoroutine("doublethrow");
      }
    }
    else
    {
      //create bullet.
      GameObject BulletObj = Instantiate(throwObj, transform);
      if (gameControl.IsHeavyDamage)
      {
        // set damage to bullet.
        BulletObj.GetComponent<Bullet>().SetDamge(player.AmountDamage * gameControl.HeavyDamageScale, player.CriticalDamage * gameControl.HeavyDamageScale);
        // change image for heavy attack.
        BulletObj.GetComponent<SpriteRenderer>().sprite = HeavyBullet;
      }
      else
      {
        BulletObj.GetComponent<Bullet>().SetDamge(player.AmountDamage, player.CriticalDamage);
      }
      BulletObj.GetComponent<Bullet>().GetAnimationCharactor(GetComponent<AnimationCharactor>());
      //set angle for throw.
      throwDir = Quaternion.Euler(0, 0, -throwAngle) * Vector2.left;
      BulletObj.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce * throwPower / 0.03f);
      //add wind force.
      BulletObj.GetComponent<Rigidbody2D>().velocity = new Vector2(gameControl.WindForce * 20, 0);
      gameControl.resetHeavyAttack();

      if (gameControl.IsDoubleAttack)
      {
        // attack 2 time.
        StartCoroutine("doublethrow");
      }
    }
  }

  // Charge force for throwing.
  private void calculateThrowForce()
  {
    //reset charge bar and set collider this player to false.
    if (pressHoldTime == 0)
    {
      col.enabled = false;
      chargeThowForce.fillAmount = 0;
    }
    pressHoldTime += Time.deltaTime * pingpongSpeed / 10f;
    // throw power be during 0 - 0.3.
    throwPower = Mathf.PingPong(pressHoldTime, 0.3f);
    chargeThowForce.fillAmount = throwPower;
  }

  private void calculateThrowForceAi()
  {//reset charge bar and set collider this player to false.
    chargeBar.gameObject.SetActive(true);

    if (pressHoldTime == 0)
    {
      col.enabled = false;
      chargeThowForce.fillAmount = 0;
    }

    if (aiEnemy != null)
    {
      if (aiEnemy.IsTurn)
      {
        //calcurate throwing force with wind.
        float aiPressHoldtime = aiEnemy.CalculateForceWithWind();
        if (throwPower < aiPressHoldtime)
        {
          pressHoldTime += Time.deltaTime * pingpongSpeed / 10f;
          throwPower = Mathf.PingPong(pressHoldTime, 0.3f);
          chargeThowForce.fillAmount = throwPower;
        }
        else
        {
          //reset random.
          aiEnemy.ResetRandomAlreadyChange();
          chargeBar.gameObject.SetActive(false);
          gameControl.setCloseInteractiveBtn();
          //throw object.
          ThrowObject();
          Invoke("resetPara", 3f);
          aiEnemy.SetAiTurn(false);
        }
      }
    }
  }

  private void resetPara()
  {
    col.enabled = true;
    pressHoldTime = 0;
    throwPower = 0;
  }

  //check mouse over on ui.
  private bool isPointerOverUIObject()
  {
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    eventData.position = Input.mousePosition;
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, results);
    return results.Count > 0;
  }

  // Second attack
  private IEnumerator doublethrow()
  {
    yield return new WaitForSeconds(2.5f);
    gameControl.resetDoubleAttack();
    if (gameControl.CurrentPlayerSide == PlayerSide.PigRich)
    {
      GameObject BulletObj = Instantiate(throwObj, transform);
      BulletObj.GetComponent<Bullet>().SetDamge(player.AmountDamage, player.CriticalDamage);
      BulletObj.GetComponent<Bullet>().GetAnimationCharactor(GetComponent<AnimationCharactor>());
      throwDir = Quaternion.Euler(0, 0, throwAngle) * Vector2.right;
      BulletObj.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce * throwPower / 0.03f);
      BulletObj.GetComponent<Rigidbody2D>().velocity = new Vector2(gameControl.WindForce * 20, 0);
    }
    else
    {
      GameObject BulletObj = Instantiate(throwObj, transform);
      BulletObj.GetComponent<Bullet>().SetDamge(player.AmountDamage, player.CriticalDamage);
      BulletObj.GetComponent<Bullet>().GetAnimationCharactor(GetComponent<AnimationCharactor>());
      throwDir = Quaternion.Euler(0, 0, -throwAngle) * Vector2.left;
      BulletObj.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce * throwPower / 0.03f);
      BulletObj.GetComponent<Rigidbody2D>().velocity = new Vector2(gameControl.WindForce * 20, 0);
    }
  }
}