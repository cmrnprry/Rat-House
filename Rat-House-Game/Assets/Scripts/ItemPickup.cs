using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    public Animator dialogueBoxAnim;
    public Animator itemGetAnim;

    public TextMeshProUGUI itemText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public string[] itemComments;

    public ItemType itemType;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {            
            {
                dialogueBoxAnim.SetBool("isOpen", true);
                dialogueText.text = itemComments[Random.Range(0, itemComments.Length - 1)];

                StartCoroutine(PickupAndClose());
            }
        }        
    }

    IEnumerator PickupAndClose()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        //close the text box if it's open...
        if (dialogueBoxAnim.GetBool("isOpen") == true && Input.GetButton("SelectAction"))
        {
            Debug.Log("Pick up the item and close the dialogue box");
            dialogueBoxAnim.SetBool("isOpen", false);

            CombatController.instance.itemList.Add(new Items(itemType, 1, 10));
            GameManager.instance.CollapseItemList(CombatController.instance.itemList);

            itemText.text = this.gameObject.name + " has been added to your record";
            itemGetAnim.SetTrigger("textPopup");
            
            Destroy(this.gameObject);
        }
        yield return null;
    }
}
