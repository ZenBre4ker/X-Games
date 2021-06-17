using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntrySelectable : MonoBehaviour, ISelectHandler,IDeselectHandler
{
    public EntrySelected entrySelected;
    public EntryDeSelected entryDeSelected;
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log($"{gameObject.name} was Selected.");
        entrySelected?.Invoke(gameObject.name);
    }
    public void OnDeselect(BaseEventData data)
    {
        //Debug.Log($"{gameObject.name} was Deselected");
        entryDeSelected?.Invoke(gameObject.name);
    }
}
