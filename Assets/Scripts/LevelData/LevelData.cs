using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        public int gridX;
        public int gridY;
        public int numberMoves;
        public int requiredMatches;
        [FormerlySerializedAs("timer")] public float timeLeft;
    }
}
