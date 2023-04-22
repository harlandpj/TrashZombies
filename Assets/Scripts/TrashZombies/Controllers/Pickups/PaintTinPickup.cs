using System.Collections;
using System.Collections.Generic;
using TrashZombies.Pickups;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

/// <summary>
/// Wine bottle collectable derived from PickupBase class
/// </summary>
public class PaintTinPickup : PickupBase
{
    [SerializeField]
    AudioClip paintTinDrop;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
       
        Debug.Log("Entered OnTriggerEnter in Paint Tin Pickup!");
    }
}