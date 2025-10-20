using System;
using System.Collections.Generic;
using Sonat.Enums;
using UnityEngine;

namespace MyGame.SkewerJam.Gameplay.LogicOrder.Configs
{
    [CreateAssetMenu(fileName = "LogicOrderConfigSO", menuName = "MyGame/SkewerJam/Gameplay/LogicOrder/Configs/LogicOrderConfigSO")]
    public class LogicOrderConfigSO : ScriptableObject
    {
        public List<Sequence> listSequenceConfigs;

        public SequenceConfigSO GetSequenceConfigSO(int sequenceLogicOrderIndex, LevelDifficulty difficulty)
        {
            var sequence = listSequenceConfigs[sequenceLogicOrderIndex];
            var difficultyAndSequence = sequence.difficultyAndSequences.Find(e => e.difficulty == difficulty);
            return difficultyAndSequence.sequence;
        }
    }

    [Serializable]
    public class Sequence
    {
        public List<DifficultyAndSequence> difficultyAndSequences;
    }

    [Serializable]
    public class DifficultyAndSequence
    {
        public LevelDifficulty difficulty;
        public SequenceConfigSO sequence;
    }
}