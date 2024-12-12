using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _instruction;
    [SerializeField] private GameObject _demoTiles, _okButton;

    private int _index;
    private string[] _lines = {
        "Welcome to your first game.\nI'll get you through the basics.",
        "Click and drag a line of letters to form a word.\nClick the tick button to confirm.",
        "You will get points equal to the combined score of each letter times the total letters.",
        "You can choose up to 3 powerups to help you during the game.",
        "There will be a timer.\nUse your time wisely.",
        "Swipe the following tiles to start the game."
    };

    private void OnEnable()
    {
        _instruction.text = string.Empty;
        Notifier.Instance.StopCountdown();
        StartDialogue();

        _okButton.SetActive(true);
        _demoTiles.SetActive(false);
    }

    private void OnDisable()
    {
        Notifier.Instance.BeginCountdown();
    }

    private void StartDialogue()
    {
        _index = 0;
        Timing.RunCoroutine(TypeLine());
    }

    private IEnumerator<float> TypeLine()
    {
        foreach (var c in _lines[_index].ToCharArray())
        {
            _instruction.text += c;
            yield return Timing.WaitForOneFrame;
        }
    }

    public void NextLine()
    {
        if (_instruction.text == _lines[_index])
        {
            if (_index < _lines.Length - 1)
            {
                _index++;
                _instruction.text = string.Empty;
                Timing.RunCoroutine(TypeLine());
            }
            else
            {
                _okButton.SetActive(false);
                _demoTiles.SetActive(true);
                _demoTiles.transform.localScale = Vector3.zero;
                _demoTiles.transform.DOScale(Vector3.one, 0.5f).OnComplete(() => DemoTiles.Instance.CursorAnimation());
            }
        }
        else
        {
            Timing.KillCoroutines();
            _instruction.text = _lines[_index];
        }
    }
}
