using UnityEngine;

[CreateAssetMenu(fileName = "TileConfig", menuName = "TileStats")]
public class TileConfig : ScriptableObject
{
    public char Letter;
    public int Score;
    public Color Color => GetColor();

    private Color GetColor()
    {
        return Letter switch
        {
            'A' or 'E' or 'I' or 'O' or 'U' or 'L' or 'N' or 'S' or 'T' or 'R' => new Color(1, 1, 1),
            'D' or 'G' => new Color(253.0f / 255f, 221.0f / 255f, 49.0f / 255f),
            'B' or 'C' or 'M' or 'P' => new Color(253.0f / 255f, 167.0f / 255f, 49.0f / 255f),
            'F' or 'H' or 'V' or 'W' or 'Y' => new Color(236.0f / 255f, 94.0f / 255f, 48.0f / 255f),
            'K' => new Color(190.0f / 255f, 28.0f / 255f, 185.0f / 255f),
            'J' or 'X' => new Color(34.0f / 255f, 153.0f / 255f, 146.0f / 255f),
            'Q' or 'Z' => new Color(97.0f / 255f, 172.0f / 255f, 126.0f / 255f),
            _ => new Color(0.0f, 0.0f, 0.0f)
        };
    }
}