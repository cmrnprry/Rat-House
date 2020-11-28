using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryMenu : MonoBehaviour
{
    [Header("Item Info")]
    public Sprite[] itemImages;
    public Sprite[] statsEffectImages;

    [Header("Items")]
    public Image item;
    public Image stausEffect;
    public TextMeshProUGUI amount;

    [Header("Profile Info")]
    public Sprite[] ProfileImages;
    [TextArea(3, 5)]
    public string[] profileInfo;

    [Header("Profile")]
    public Image profile;
    public TextMeshProUGUI profileName;
    public TextMeshProUGUI description;


    [Header("Iventory Parents")]
    public GameObject invProfile;
    public GameObject invItems;
    public GameObject highlight;

    [Header("Tabs")]
    public GameObject Item;
    public GameObject Profile;

    private EventSystem ev;
    private int currItem = 0;
    private int currProfile = 0;

    private List<Items> items;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OpenInventory()
    {
        ev = FindObjectOfType<EventSystem>();
        items = CombatController.instance.itemList;
        invProfile.SetActive(false);
        invItems.SetActive(true);
        Item.GetComponent<Button>().Select();


        SetItem();

        StartCoroutine(TurnOff());
        StartCoroutine(Select());

    }

    /*  Methods to set what the page shoule show   */
    void SetItem()
    {
        amount.text = "" + items[currItem].count;
        stausEffect.sprite = GetEffect(items[currItem].effect);
        item.sprite = GetItem(items[currItem].item);
    }

    void SetProfile()
    {
        string[] text = profileInfo[currProfile].Split(':');
        profileName.text = text[0];
        description.text = text[1];
        profile.sprite = ProfileImages[currProfile];
    }

    /*  Getters for the item and status effect   */
    Sprite GetItem(ItemType type)
    {
        if (type.ToString() == "Calmy_Tea")
        {
            return itemImages[0];
        }
        else if (type.ToString() == "Plastic_Utensils")
        {
            return itemImages[1];
        }
        else if (type.ToString() == "Hot_Coffee")
        {
            return itemImages[2];
        }
        else if (type.ToString() == "Pams_Fruitcake")
        {
            return itemImages[3];
        }
        else if (type.ToString() == "Jims_Lunch")
        {
            return itemImages[4];
        }

        return itemImages[5];
    }

    Sprite GetEffect(StatusEffect effect)
    {

        if (effect.ToString() == "Cures_Burn")
        {
            return statsEffectImages[0];
        }
        else if (effect.ToString() == "Cures_Poison")
        {
            return statsEffectImages[1];
        }
        else if (effect.ToString() == "Burn")
        {
            return statsEffectImages[0];
        }
        else if (effect.ToString() == "Poison")
        {
            return statsEffectImages[1];
        }
        else if (effect.ToString() == "Bleed")
        {
            return statsEffectImages[2];
        }

        return statsEffectImages[0];
    }

    bool PressedArrow()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            return true;
        }
        return false;
    }

    IEnumerator Select()
    {
        yield return new WaitUntil(() => Input.GetKey(KeyCode.Return) || PressedArrow());

        if (ev.currentSelectedGameObject == null)
        {
            GameObject obj = invProfile.activeSelf ? Profile : Item;
            Debug.Log(obj.name);
            ev.SetSelectedGameObject(obj);
            obj.GetComponent<Button>().Select();
        }

        switch (ev.currentSelectedGameObject.name)
        {
            case "Left":
                CycleLeft();
                break;
            case "Right":
                CycleRight();
                break;
            case "Item Tab":
                SelectItemTab(ev.currentSelectedGameObject);
                break;
            case "Profile Tab":
                SelectProfileTab(ev.currentSelectedGameObject);
                break;
        }

        yield return new WaitForSecondsRealtime(.15f);
        StartCoroutine(Select());
    }

    public void SelectProfileTab(GameObject obj)
    {
        obj.GetComponent<Button>().Select();
        SetProfile();
        invProfile.SetActive(true);
    }

    public void SelectItemTab(GameObject obj)
    {
        obj.GetComponent<Button>().Select();
        SetItem();
        invProfile.SetActive(false);
    }

    public void CycleLeft()
    {
        if (invProfile.activeSelf)
        {
            currProfile = (currProfile < ProfileImages.Length - 1) ? (currProfile + 1) : 0;
            SetProfile();
        }
        else
        {
            currItem = (currItem < items.Count - 1) ? (currItem + 1) : 0;
            SetItem();
        }
    }

    public void CycleRight()
    {
        if (invProfile.activeSelf)
        {
            currProfile = (currProfile != 0) ? (currProfile - 1) : (ProfileImages.Length - 1);
            SetProfile();
        }
        else
        {
            currItem = (currItem != 0) ? (currItem - 1) : (items.Count - 1);
            SetItem();
        }
    }

    public void CloseInventory()
    {
        highlight.SetActive(true);
    }


    /*   Set highlight for mouse hover   */

    public void SetHighlightOn(Image i)
    {
        i.color = new Color32(255, 255, 225, 255);
    }

    public void SetHighlightOff(Image i)
    {
        i.color = new Color32(255, 255, 225, 0);
    }


    /*  Turns off the intial highlight that i have on bc bad code :(   */
    IEnumerator TurnOff()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow));
        TurnOffHighlight();
    }

    public void TurnOffHighlight()
    {
        highlight.SetActive(false);
    }
}
