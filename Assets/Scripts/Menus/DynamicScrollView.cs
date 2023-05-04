using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollView : MonoBehaviour
{
    [Tooltip ("Drag Scrollview content object into here")]
    [SerializeField]
    private Transform scrollViewcontent;

    [SerializeField]
    private GameObject linesContainer; // container object containing TAGGED TMP_Text items

    /// <summary>
    /// Populates Scrollview entries
    /// </summary>
    private void Start()
    {
        // find display items and populate view
        TMP_Text[] displayItems = linesContainer.GetComponentsInChildren<TMP_Text>();

        // sort list by Asset type TAG  then by text entry
        var listSorted = displayItems.OrderBy(p => p.tag).ThenBy(
                        p => p.GetComponent<TMP_Text>().text); 

        foreach (TMP_Text displayEntry in listSorted)
        {
            TMP_Text newTextEntry = Instantiate(displayEntry,scrollViewcontent);
        }
    }
}
