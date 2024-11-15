using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelData
{
    [CreateAssetMenu(menuName = "Level Data", fileName = "Level Data")]
    public class LevelData : ScriptableObject
    {
        public List<LevelInformation> LevelInfos = new List<LevelInformation>();
    }
    
    [Serializable]
    public struct LevelInformation
    {
        public int gridSize;
        public float timer;
    }
}