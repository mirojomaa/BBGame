using UnityEngine;
using NaughtyAttributes;
public class ResetPlayerPrefs : MonoBehaviour
{
    [Button("Reset HextileWinCons")] public void Reset()
    {
        PlayerPrefs.SetInt("WinConMissions", 0);
        PlayerPrefs.SetInt("WinConHex", 0);
        PlayerPrefs.SetInt("WinConPoints", 0);
    }
    [Button("Reset Highscore")] public void ResetHighscore() =>PlayerPrefs.SetFloat("Highscore", 0);
}