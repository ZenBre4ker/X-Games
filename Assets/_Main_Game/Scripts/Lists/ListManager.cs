using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void EntrySelected(string _name);
public delegate void EntryDeSelected(string _name);
public class ListManager : MonoBehaviour
{
    public GameObject hostEntryPrefab;

    public EntrySelected entrySelected=null;
    public EntryDeSelected entryDeSelected=null;

    private float contentHeight=120f;
    private int entries=0;
    private List<string> hostNames;
    private List<GameObject> hostEntries;

    private Transform content;

    // Start is called before the first frame update
    void Start()
    {
        hostNames = new List<string>();
        hostEntries = new List<GameObject>();

        content = transform.GetChild(0).GetChild(0); 
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, entries * contentHeight); 
    }
    public void addEntry(string _name)
    {
        entries += 1;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, entries * contentHeight);
        GameObject _hostEntry = GameObject.Instantiate(hostEntryPrefab, content);
        _hostEntry.name = _name;
        _hostEntry.GetComponent<Text>().text = _name;

        hostEntries.Add(_hostEntry);
        hostNames.Add(_name);

        EntrySelectable _selectable = _hostEntry.GetComponent<EntrySelectable>();
        _selectable.entrySelected = entrySelected;
        _selectable.entryDeSelected = entryDeSelected;
    }
    public void deleteEntry(int _entry)
    {
        if (_entry < hostNames.Count)
        {
            entries -= 1;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, entries * contentHeight);
            GameObject _hostEntry = hostEntries[_entry];

            hostEntries.RemoveAt(_entry);
            hostNames.RemoveAt(_entry);

            Destroy(_hostEntry);
        }
    }

    public void deleteEntry(string _name)
    {
        if (hostNames.Contains(_name))
        {
            int hostIndex = hostNames.IndexOf(_name);
            deleteEntry(hostIndex);
        }
    }
}
