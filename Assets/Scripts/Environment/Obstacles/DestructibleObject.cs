using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    private PlayerMovement _playerMove;

    [SerializeField] AudioSource destroySound;
    [Range(0.01f, 1f)]
    [SerializeField] private float fuelSub = 0.1f;

    private int _colliderID;
    private MeshCollider _myCollider;
    private MeshFilter _myMeshFilter;
    private ConcurrentDictionary<uint, byte> _faceIndices;
    private Mesh _originalMesh;
    private Mesh _myMesh;
    private int[] _myTris;
    

    void Start()
    {
        _playerMove = PlayerMovement.Instance;

        _myCollider = GetComponent<MeshCollider>();
        _myCollider.hasModifiableContacts = true;
        _colliderID = _myCollider.GetInstanceID();

        _myMeshFilter = GetComponent<MeshFilter>();
        _originalMesh = _myMeshFilter.sharedMesh;
        _myMeshFilter.sharedMesh = Instantiate(_originalMesh);
        _myCollider.sharedMesh = _myMeshFilter.sharedMesh;

        _myMesh = _myCollider.sharedMesh;
        _myTris = _myMesh.triangles;

        _faceIndices = new();

        Physics.ContactModifyEvent += OnContactModifyEvent;

        if (!_playerMove) Debug.LogError("SpeedPlane Error: No PlayerMovement found");
    }

    void OnTriggerEnter( Collider collider )
    {
        _playerMove.ReduceFuel(fuelSub);

        destroySound.Play();

        if (Input.GetKeyDown(KeyCode.F)) ResetMesh();
    }

    // private void FixedUpdate()
    // {
    //     if (_faceIndices.IsEmpty) return;
    //
    //     Mesh myMesh = _myMeshFilter.sharedMesh;
    //     
    //     List<int> triIndices = new List<int>(myMesh.triangles);
    //
    //     foreach (var faceIndex in _faceIndices)
    //     {
    //         int faceInt = (int)faceIndex.Key;
    //
    //         if (faceInt < triIndices.Count)
    //         {
    //             triIndices.RemoveRange(faceInt, 3);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("DestructibleObject Warning: Invalid face index found");
    //         }
    //     }
    //
    //     myMesh.triangles = triIndices.ToArray();
    //
    //     _myMeshFilter.sharedMesh = myMesh;
    //     _myCollider.sharedMesh = myMesh;
    //
    //     _faceIndices.Clear();
    // }

    private void FixedUpdate()
    {
        _myMesh.triangles = _myTris;
        _myMeshFilter.sharedMesh = _myMesh;
        _myCollider.sharedMesh = _myMesh;
    }

    private void OnContactModifyEvent(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
    {
        foreach (ModifiableContactPair pair in pairs)
        {
            if (pair.otherColliderInstanceID == _colliderID)
            {
                for (int i = 0; i < pair.contactCount; i++)
                {
                    pair.IgnoreContact(i);
                    
                    _faceIndices.TryAdd(pair.GetFaceIndex(i) * 3, byte.MaxValue);
                }
        
                List<int> triIndices = new List<int>(_myTris);

                foreach (var faceIndex in _faceIndices)
                {
                    int faceInt = (int)faceIndex.Key;
                    
                    if (faceInt < triIndices.Count)
                    {
                        triIndices.RemoveRange(faceInt, 3);
                    }
                    else
                    {
                        Debug.LogWarning("DestructibleObject Warning: Invalid face index found");
                    }
                }
                
                _myTris = triIndices.ToArray();

                _faceIndices.Clear();
            }
        }
    }

    // Restore the mesh to its whole form
    private void ResetMesh()
    {
        _myMeshFilter.sharedMesh = Instantiate(_originalMesh);
        _myCollider.sharedMesh = _myMeshFilter.sharedMesh;
    }

}
