using UnityEditor;
using UnityEngine;

public class RemovePlayerPrefs : EditorWindow
{
    [MenuItem("Tools/Remove Player Prefs")]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player Prefs Deleted!");
    }
}
