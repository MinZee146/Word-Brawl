using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NameRandomizer : Singleton<NameRandomizer>
{
    private TextAsset _nameText;
    private List<string> _nameList = new List<string>();

    public void Initialize()
    {
        Addressables.LoadAssetAsync<TextAsset>("names").Completed += OnNamesLoaded;
    }

    private void OnNamesLoaded(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _nameText = handle.Result;

            using var reader = new StringReader(_nameText.text);
            while (reader.ReadLine() is { } line)
            {
                _nameList.Add(line);
            }

            Debug.Log("Names loaded and processed. Total names: " + _nameList.Count);
        }
        else
        {
            Debug.LogError("Failed to load name asset from Addressables.");
        }
    }

    public string GetRandomOpponent()
    {
        return _nameList[Random.Range(0, _nameList.Count)];
    }

    public void UnloadNames()
    {
        Addressables.Release(_nameText);
        _nameList.Clear();
    }
}
