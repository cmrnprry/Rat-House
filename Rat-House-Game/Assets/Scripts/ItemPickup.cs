using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemType itemType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        CombatController.instance.itemList.Add(new Items(itemType, 1, 10));
        GameManager.instance.CollapseItemList(CombatController.instance.itemList);
        Destroy(gameObject);
    }
}
