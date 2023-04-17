using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TrashZombies.Pickups;

/// <summary>
/// Scriptable Object data container defining common stats for Pickups (name, score value, icon used)
/// </summary>
[CreateAssetMenu(fileName = "New Pickup Stats", menuName = "Pickup Creation/Pickup Stats", order = 51)]
public class PickupStats : ScriptableObject
{
    [SerializeField]
    private string pickupName; // pickup name (for UI display)

    [SerializeField]
    private int pickupValue; // score value

    [SerializeField]
    private Sprite icon; // icon for UI display (later on)

    // member accessors
    public string PickupName
    {
        get
        {
            return pickupName;
        }
    }

    public int PickupValue
    {
        get
        {
            return pickupValue;
        }
    }

    public Sprite Sprite
    {
        get
        {
            return icon;
        }
    }
    
    public void PrintMessage()
    {
        Debug.Log("The " + pickupName + " pickup data has been loaded.");
    }
}
