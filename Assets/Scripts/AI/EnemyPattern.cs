using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Pattern", menuName = "Enemy Pattern")]
public class EnemyPattern : ScriptableObject
{
    private List<GameObject> _positions;

    public void Instantiate()
    {
        if (_positions == null)
        {
            _positions = new();
        }
        else
        {
            _positions.Clear();
        }
    }

    public void AddPosition(Vector3 pos, GameObject template)
    {
        GameObject newObj = Instantiate(template, pos, Quaternion.identity);
        newObj.name = "Target " + _positions.Count + "\n" + pos;
        _positions.Add(newObj);
    }

    public Vector3 GetPosition(int i)
    {
        return _positions[i].transform.position;
    }

    public int GetCount()
    {
        return _positions.Count;
    }
}
