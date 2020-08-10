using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeReference]
    private float _maxHealth;

    [SerializeReference]
    private float _baseAcuracy;

    [SerializeReference]
    private float _baseAttack;

    private bool _turnOver = false;

    public string effectName;
    private ParticleSystem _attackAnim;

    private void Start()
    {
        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = transform.position;
        //Instasiate the enmy of Type
        // GameObject enemy = Instantiate(Resources.Load("Enemies/" + e.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);
    }

    //Handles a single enemy's turn
    public void AttackPlayer(EnemyType e)
    {
        switch (e)
        {
            case EnemyType.Coffee:
                Debug.Log("Coffe man attack");
                StartCoroutine(CoffeeAttack());
                break;
            case EnemyType.Water_Cooler:
                Debug.Log("water man attack");
                StartCoroutine(WaterAttack());
                break;
            case EnemyType.Intern:
                Debug.Log("Intern attack");
                StartCoroutine(InternAttack());
                break;
            case EnemyType.Tutorial_Intern:
                Debug.Log("Tutorial Intern attack");
                StartCoroutine(TutorialInternAttack());
                break;
            default:
                Debug.LogError("Error in Enemy Attack");
                break;
        }

    }

    //Handles attacks for the Coffee Man
    IEnumerator CoffeeAttack()
    {
        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        //while the animation is playing wait

        while (!_attackAnim.isStopped)
        {
            Debug.Log("attack animation Over: " + _attackAnim.isStopped);
            yield return null;
        }

        yield return new WaitForEndOfFrame();

       _turnOver = true;
    }


    //Handles attacks for Intern
    IEnumerator InternAttack()
    {
        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        //while the animation is playing wait

        while (!_attackAnim.isStopped)
        {
            Debug.Log("attack animation Over: " + _attackAnim.isStopped);
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        _turnOver = true;
    }

    IEnumerator WaterAttack()
    {
        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        //while the animation is playing wait

        while (!_attackAnim.isStopped)
        {
            Debug.Log("attack animation Over: " + _attackAnim.isStopped);
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        _turnOver = true;
    }


    //Handles attacks for Intern
    IEnumerator TutorialInternAttack()
    {
        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        //while the animation is playing wait

        while (!_attackAnim.isStopped)
        {
            Debug.Log("attack animation Over: " + _attackAnim.isStopped);
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        _turnOver = true;
    }

    public float GetBaseAttack()
    {
        return _baseAttack;
    }

    //Getter for the player starting heath
    public float GetStartingHealth()
    {
        return _maxHealth;
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
