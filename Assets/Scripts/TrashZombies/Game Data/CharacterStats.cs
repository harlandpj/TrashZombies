using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object data container defining common stats for Player AND NPC (some may not be used in Player) characters
/// </summary>
[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character Creation/Character Stats", order = 51)]
public class CharacterStats : ScriptableObject
{
    [SerializeField]
    private string characterName; // character name

    [SerializeField]
    private int attackDamage; // damage dealt when attacking

    [SerializeField]
    private float normalSpeed; // movement speed (normal walk)

    [SerializeField]
    private float sprintSpeed; // movement speed (sprint)

    [SerializeField]
    private float eyesightDistance; // distance character can see (not applicable to Player)

    [SerializeField]
    private int maxHealth; // maximum health points

    [SerializeField]
    private int attackRecoveryTime; // recovery time after being attacked before can pursue/patrol again

    [SerializeField]
    private Sprite icon; // icon for UI display (later on)

    // member accessors
    public string CharName
    {
        get
        {
            return characterName;
        }
    }

    public int AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }

    public float NormalSpeed
    {
        get
        {
            return normalSpeed;
        }
    }
    public float SprintSpeed
    {
        get
        {
            return sprintSpeed;
        }
    }

    public float EyesightDistance
    {
        get
        {
            return eyesightDistance;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }

    public int AttackRecoveryTime
    {
        get
        {
            return attackRecoveryTime;
        }
    }

    public void PrintMessage()
    {
        Debug.Log("The " + characterName + " character data has been loaded.");
    }
}
