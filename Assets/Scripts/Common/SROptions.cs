using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    [Category("Cheats")]
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}