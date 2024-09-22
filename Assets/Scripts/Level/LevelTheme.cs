using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class LevelTheme : ScriptableObject
{
    public EnviromentObject[] m_grass;
    public EnviromentObject[] m_bushes;
    public EnviromentObject[] m_trees;

    public int m_themeNum;
}
