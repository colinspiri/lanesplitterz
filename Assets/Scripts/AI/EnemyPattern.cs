using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Pattern", menuName = "Enemy Pattern")]
public class EnemyPattern : ScriptableObject
{
    private List<GameObject> _objects;
    private List<Vector3> _positions;

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

        if (_objects == null)
        {
            _objects = new();
        }
        else
        {
            _objects.Clear();
        }
    }

    public void AddPosition(Vector3 pos, GameObject template)
    {
        GameObject newObj = Instantiate(template, pos, Quaternion.identity);
        newObj.name = "Target " + _positions.Count + "\n" + pos;
        _objects.Add(newObj);
        _positions.Add(newObj.transform.position);
    }

    public Vector3 GetPosition(int i)
    {
        return _positions[i];
    }

    public int GetCount()
    {
        return _positions.Count;
    }
}
