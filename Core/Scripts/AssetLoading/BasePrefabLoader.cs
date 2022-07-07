using System.Collections.Generic;
using System.Threading.Tasks;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public class BasePrefabLoader : MonoBehaviour
{

    [SerializeField] private List<GameObject> prefabList;
    private GameObject _currentPrefab;

    // the simple way to do this
    public void ShowPrefab(int numBPrefab)
    {
        if (numBPrefab > prefabList.Count - 1 || prefabList == null || prefabList.Count == 0) return;

        if (!_currentPrefab.SafeIsUnityNull())
            Destroy(_currentPrefab);

        // instantiate and add to this gameObject
        _currentPrefab = Instantiate(prefabList[numBPrefab], gameObject.transform, true);
    }

    // we might still consider loading the prefabs using Addressables.LoadAssetAsync or Addressables.InstantiateAsync from Json
    public async Task LoadAddressablePrefab( string addressableName )
    {
       var loadHandle =  Addressables.InstantiateAsync(addressableName, gameObject.transform );
       await loadHandle.Task;
    }

    // cleanup children
    public void DestroyAll()
    {
        foreach (Transform child in transform)
        {
            Destroy(child);
        }
    }
}
