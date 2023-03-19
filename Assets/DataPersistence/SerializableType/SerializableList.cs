using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableList<T> : List<T>, ISerializationCallbackReceiver
{
    [SerializeField] private List<T> list = new List<T>();
    public void OnAfterDeserialize()
    {
        list.Clear();
        foreach (T item in this)
        {
            list.Add(item);
        }
    }

    public void OnBeforeSerialize()
    {
        this.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            this.Add(list[i]);
        }
    }
}
