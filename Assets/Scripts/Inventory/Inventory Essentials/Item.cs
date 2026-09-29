using System.Xml.Serialization;
using UnityEngine;

public class Item : MonoBehaviour
{
    //associates item with additional variables
    public ItemScript item;
    public int amount = 1;
    public bool canPickup = true;
    public Outline outline;

    private void Awake()
    {
        //starts outline disabled by default
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }
}
