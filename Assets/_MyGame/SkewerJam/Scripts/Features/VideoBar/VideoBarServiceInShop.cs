using System;
using UnityEngine;

namespace MyGame.SkewerJam.Features.VideoBar
{
    [CreateAssetMenu(fileName = "VideoBarServiceInShop", menuName = "MyGame/SkewerJam/Features/VideoBarServiceInShop")]
    public class VideoBarServiceInShop : VideoBarService
    {
        protected override string VIDEO_BAR_KEY => "VIDEO_BAR_KEY_IN_SHOP";
    }
}