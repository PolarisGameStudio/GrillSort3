using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.Modules.CardCollection
{
    [CreateAssetMenu(fileName = "CardSubmodule", menuName = "MyGame/SkewerJam/Features/CardCollection/CardSubmodule")]
    public class CardSubmodule : ScriptableObject
    {
        [SerializeField] private CardInventoryModule cardInventoryModule;

        public int GetNumCard(CardType cardType)
        {
            return 1;
        }
    }
}