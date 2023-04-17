using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrashZombies.Pickups;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

/// <summary>
/// Wine bottle collectable derived from PickupBase class
/// </summary>
public class WineBottlePickup : PickupBase
{
    [SerializeField]
    AudioClip bottleDrop;

    private void Awake()
    {
        base.Awake();
    }

    private void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Debug.Log("Entered OnTriggerEnter in Wine Bottle Pickup!");
    }
}
