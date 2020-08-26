using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    public ItemType itemType;
    public StatusEffect effectType;
    public int itialDmg;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {            
            {
                CombatController.instance.itemList.Add(new Items(itemType, 1, itialDmg, effectType));
                GameManager.instance.CollapseItemList(CombatController.instance.itemList);

                GameManager.instance.itemText.text = this.gameObject.name + " has been added to your record";
                GameManager.instance.itemGetAnim.SetTrigger("textPopup");

                this.gameObject.SetActive(false);
            }
        }        
    }
}
