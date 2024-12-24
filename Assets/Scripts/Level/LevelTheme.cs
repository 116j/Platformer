using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class LevelTheme : ScriptableObject
{
    public GameObject[] backgrounds;

    public EnviromentObject[] grass;
    public EnviromentObject[] bushes;
    public EnviromentObject[] trees;

    public SpawnValues[] enemies;
    public Trap[] ceilTraps;
    public Trap[] floorTraps;
    
    public int themeNum;
}
