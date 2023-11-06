using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
  private GameControl gameControl;
  private AnimationCharactor animationCharactor;
  public bool firstTime = true;
  private HealthBar healthBar;
  private float amountDamage;
  private float amountCriticleDamage;
  private float damage;

  private void Start()
  {
    gameControl = FindObjectOfType<GameControl>();
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    // when hit wall.
    if (collision.gameObject.tag == "Wall")
    {
      // debug for hit one more time.
      if (firstTime)
      {
        if (gameControl.IsDoubleAttack)
        {
          gameControl.resetDoubleAttack();
          animationCharactor.onMiss();
          firstTime = false;
          StartCoroutine("DestroyBulletAndChangeTurn");
        }
        else
        {
          animationCharactor.onMiss();
          firstTime = false;
          StartCoroutine("DestroyBulletAndChangeTurn");
        }
      }
    }
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (firstTime)
    {
      if (collision.gameObject.tag == "Wall")
      {
        if (gameControl.IsDoubleAttack)
        {
          gameControl.resetDoubleAttack();
          animationCharactor.onMiss();
          firstTime = false;
          Destroy(gameObject);
        }
        else
        {
          animationCharactor.onMiss();
          firstTime = false;
          StartCoroutine("DestroyBulletAndChangeTurn");
        }
      }
      //when hit player
      if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<HealthBar>().Hp > 0)
      {
        //check position between bullet and hited object for small attackin or normal attacking.
        bool normalAttack = Mathf.Abs(collision.transform.position.x - transform.position.x) > 0.5;
        SetDamgeForCalculate(normalAttack);
        if (gameControl.IsDoubleAttack)
        {
          //Get health bar from hited object.
          healthBar = collision.gameObject.GetComponent<HealthBar>();
          healthBar.calCulateDamage(damage / 2, (hp) =>
          {
            animationCharactor.onDamage(hp, normalAttack);
          });
          // debug for hit one more time.
          firstTime = false;
          Destroy(this.gameObject);
          gameControl.resetDoubleAttack();
        }
        else
        {
          if (gameControl.IsHeavyDamage)
          {
            healthBar = collision.gameObject.GetComponent<HealthBar>();
            healthBar.calCulateDamage(damage, (hp) =>
            {
              animationCharactor.onDamage(hp, normalAttack);
            });
            firstTime = false;
            Destroy(this.gameObject);
            gameControl.SwitchPlayerSide();
          }
          else
          {
            healthBar = collision.gameObject.GetComponent<HealthBar>();
            healthBar.calCulateDamage(damage, (hp) =>
            {
              animationCharactor.onDamage(hp, normalAttack);
            });

            firstTime = false;
            Destroy(this.gameObject);
            gameControl.SwitchPlayerSide();
          }
        }
      }
    }
  }

  //Destroy bullet and set player turn.
  private IEnumerator DestroyBulletAndChangeTurn()
  {
    yield return new WaitForSeconds(1f);
    if (!gameControl.IsDoubleAttack)
    {
      Destroy(this.gameObject);
      gameControl.SwitchPlayerSide();
    }
  }

  //Get animation from hited object.
  public void GetAnimationCharactor(AnimationCharactor animationCharactor)
  {
    this.animationCharactor = animationCharactor;
  }

  //set damage to bullet.
  public void SetDamge(float damage, float CriticalDamage)
  {
    amountDamage = damage;
    amountCriticleDamage = CriticalDamage;
  }

  private void SetDamgeForCalculate(bool isNormalAttack)
  {
    if (isNormalAttack)
    {
      damage = amountDamage;
    }
    else
    {
      damage = amountCriticleDamage;
    }
  }
}