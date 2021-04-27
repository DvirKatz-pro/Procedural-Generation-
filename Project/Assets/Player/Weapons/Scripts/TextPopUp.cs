using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float damage = 0;
    private int durability = 0;
    private string weaponName = "";
    private float currentWeaponDamage;
    private WeaponStats weaponStats;
    private List<Text> textList;

    public void Start()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Transform image = this.transform.Find("Image");
        Transform[] textTransform = new Transform[image.childCount];
        int i = 0;
        foreach (Transform child in image)
        {

            textTransform[i] = child;
            i++;
        }
        Debug.Log(textTransform[0].gameObject.name);
        Text text = textTransform[0].gameObject.GetComponent<Text>();
        //currentWeaponDamage = GameObject.Find("Player").GetComponent<Inventory>().currentWeapon.GetComponent<WeaponStats>().damage;
        text.text = string.Format("Name: {0}", weaponName);
        Debug.Log(textTransform[1].gameObject.name);
        text = textTransform[1].gameObject.GetComponent<Text>();
        text.text = string.Format("Damage: {0}  ({1})", damage, currentWeaponDamage);
        if (currentWeaponDamage > damage)
        {
            text.color = Color.red;
        }
        else if (currentWeaponDamage < damage)
        {
            text.color = Color.green;
        }
        else
        {
            text.color = Color.white;
        }
        text = textTransform[2].gameObject.GetComponent<Text>();
        text.text = string.Format("Durability: {0}%", durability);
        Transform imageTransform = this.transform.Find("Image");
        imageTransform.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Transform imageTransform = this.transform.Find("Image");
        imageTransform.gameObject.SetActive(false);
    }
    public void setText(string m_name, int m_damage, int m_durability)
    {
        damage = m_damage;
        weaponName = m_name;
        durability = m_durability;
    }
    public string getName()
    {
        return weaponName;
    }
    public float getDamage()
    {
        return damage;
    }

}
