using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjItem : MonoBehaviour
{
    [Header("������")]
    public Item item;
    [Header("������ �̹���")]
    public Sprite itemImage;

    void Start()
    {
        itemImage = item.itemImage;
    }
    public Item ClickItem()
    {
        return this.item;
    }   
}
