using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{   
    public Animator itemGetAnim;

    public TextMeshProUGUI itemText;

    public ItemType itemType;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {            
            {
                CombatController.instance.itemList.Add(new Items(itemType, 1, 10));
                GameManager.instance.CollapseItemList(CombatController.instance.itemList);

                itemText.text = this.gameObject.name + " has been added to your record";
                itemGetAnim.SetTrigger("textPopup");

                Destroy(this.gameObject);
            }
        }        
    }
}
