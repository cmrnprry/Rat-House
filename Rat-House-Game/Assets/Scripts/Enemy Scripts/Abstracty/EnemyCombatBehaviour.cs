using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class EnemyCombatBehaviour : MonoBehaviour
{
    [SerializeReference]
    protected float _maxHealth;
    public float _currentHealth;

    //status effect tracking
    protected StatusEffect effect;
    public bool hasEffect = false;
    protected int turnsUntilEffectOver;

    [SerializeReference]
    protected float _baseAttack;

    [HideInInspector]
    public Slider healthSlider;

    [Header("Attacks")]
    //Number of attacks and chances of said attacks hitting. They follow the order in the spread sheet
    public int numberOfAttacks;

    protected bool _turnOver = false;

    [Header("Animations")]
    protected Animator anim;

    public string effectName;
    protected ParticleSystem _attackAnim;

    public abstract IEnumerator AttackPlayer();

    public int CalculateChance(int upperbound)
    {
        return Random.Range(0, upperbound);
    }

    public void SetStatusEffect(Items item)
    {
        effect = item.effect;
        hasEffect = true;

        Color color = new Color();
        ColorUtility.TryParseHtmlString(CombatController.instance.GetColor(effect), out color);
        Debug.Log("Color: " + color.ToString());
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

        //For now, they will all last 3 turns
        turnsUntilEffectOver = 3;
    }

    public void UpdateEffect()
    {
        if (hasEffect)
        {
            turnsUntilEffectOver -= 1;
            UpdateHealth((int)effect);

            if (turnsUntilEffectOver <= 0)
                RemoveEffect();
        }
    }

    public void UpdateHealth(float dmg)
    {
        _currentHealth -= dmg;

        healthSlider.value = (_currentHealth / _maxHealth);
    }

    public void RemoveEffect()
    {
        hasEffect = false;
        effect = StatusEffect.None;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }

    //set triggers
    public void EnemyHit()
    {
        anim.SetTrigger("Hit");
    }

    public void Idle()
    {
        Debug.Log("idle");
        anim.SetTrigger("Idle");
    }

    public void EnemyDeath()
    {
        anim.SetTrigger("Dead");
    }

    //Getter for the player starting heath
    public float GetStartingHealth()
    {
        return _maxHealth;
    }

    public float GetBaseAttack()
    {
        return _baseAttack;
    }

    public bool IsTurnOver()
    {
        return _turnOver;
    }

    public void SetIsTurnOver(bool over)
    {
        _turnOver = over;
    }
}
