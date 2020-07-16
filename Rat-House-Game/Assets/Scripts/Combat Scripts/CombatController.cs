using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


enum ActionType
{
    Basic_Attack = 10,
    Item = 1,
}


public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public bool inBattle = false;

    [SerializeField]
    private List<ActionType> _actionList;
    private int _selected = 0;



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ResetBattle()
    {
        _selected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (_canChoose)
        //{
        //    if (Input.GetButtonDown("Up"))
        //    {
        //        Debug.Log("prss Up");

        //        if (_selected == 0)
        //        {
        //            _selected = _actionList.Count - 1;
        //        }
        //        else
        //        {
        //            _selected--;
        //        }
        //    }
        //    else if (Input.GetButtonDown("Down"))
        //    {
        //        Debug.Log("prss Down");
        //        if (_selected == _actionList.Count - 1)
        //        {
        //            _selected = 0;
        //        }
        //        else
        //        {
        //            _selected++;
        //        }
        //    }

        //    ShowSelectedAction();
        //}
    }

    public IEnumerator ChooseAction()
    {
        if (Input.GetButton("Up"))
        {
            if (_selected == 0)
            {
                _selected = _actionList.Count - 1;
            }
            else
            {
                _selected--;
            }
        }
        else if (Input.GetButton("Down"))
        {
            if (_selected == _actionList.Count - 1)
            {
                _selected = 0;
            }
            else
            {
                _selected++;
            }
        }

        if (Input.GetButton("SelectAction"))
        {
            switch (_actionList[_selected])
            {
                case ActionType.Basic_Attack:
                    StartBasicAttack();
                    break;
                case ActionType.Item:
                    Debug.Log("Open Item Menu");
                    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }

            yield break;
        }

        ShowSelectedAction();

        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(ChooseAction());
    }

    void ShowSelectedAction()
    {
        Debug.Log(_actionList[_selected]);
    }


    void StartBasicAttack()
    {
        Debug.Log("Wait until second beat to start rhytm");
    }
}
