using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDictionary : SingletonPersistent<GameDictionary>
{
    private HashSet<string> _words = new();
    private Trie _wordTrie = new();
    private TextAsset _dictText;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _dictText = (TextAsset)Resources.Load("ospd", typeof(TextAsset));
        using var reader = new StringReader(_dictText.text);

        while (reader.ReadLine() is { } line)
        {
            _words.Add(line.ToUpper());
            _wordTrie.Insert(line.ToUpper());
        }
    }

    public bool CheckWord(string word)
    {
        return _words.Contains(word);
    }

    public bool IsPrefix(string prefix)
    {
        return _wordTrie.IsPrefix(prefix.ToUpper());
    }
}