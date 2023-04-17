using System.Collections;
using System.Collections.Generic;
using TrashZombies.Pickups;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using TrashZombies.Pickups;

/// <summary>
/// Wine bottle collectable derived from PickupBase class
/// </summary>
public class PaintTinPickup : PickupBase
{
    [SerializeField]
    AudioClip paintTinDrop;

    private void Awake()
    {
        base.Awake();
    }

    private void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
       
        Debug.Log("Entered OnTriggerEnter in Paint Tin Pickup!");
    }
}