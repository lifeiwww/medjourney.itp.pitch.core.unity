using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using UnityEngine;

public class ProgressBarStrip : MonoBehaviour
{
    [SerializeField] private int numBlocks;
    [SerializeField] private GameObject blockPrefabGameObject;
    [SerializeField] private GameObject blockHolder;

    private readonly List<ProgressBarBlock> _blocks = new List<ProgressBarBlock>();
    private int _previousProgress = 0;

    public void SetNumBlocks( int num )
    {
        if (_blocks.Count == num) return;
        _blocks.Clear();
        Log.Verbose($"_blocks number {_blocks.Count}");
        foreach (Transform child in blockHolder.transform)
            Destroy(child.gameObject);
        PopulateBar(num);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SetProgress(0f);

        if (Input.GetKeyDown(KeyCode.Alpha9))
            SetProgress(1f);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetProgress(4);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SetProgress(0.5f);

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SetNumBlocks(7);

        if (Input.GetKeyDown(KeyCode.Alpha8))
            SetNumBlocks(8);
    }

    private void PopulateBar(int num)
    {
        for (var i=0; i<num; i++)
        {
            // The prefab needs to have this component with colors preset
            var barBlock = Instantiate(blockPrefabGameObject, blockHolder.transform);
            var block = barBlock.GetComponent<ProgressBarBlock>();
            _blocks.Add(block);
        }
    }

    public void SetProgress(float percent)
    {
        int num = Mathf.CeilToInt(_blocks.Count * percent );
        SetProgress(num);
    }

    public void SetProgress(int num)
    {
        // avoid setting progress numerous times and cause unwanted tweens
        if (num == _previousProgress) return;

        _previousProgress = num;
        for (var i = 0; i < _blocks.Count; i++)
        {
            var active = i < num;
            _blocks[i].Activate(active);
        }

    }
}
