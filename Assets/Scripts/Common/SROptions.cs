using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    [Category("Debug")]
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}