using UnityEngine;

namespace MyGame.SkewerJam.Features.VideoBar
{

    [CreateAssetMenu(fileName = "VideoBarServiceAtHome", menuName = "MyGame/SkewerJam/Features/VideoBarServiceAtHome")]
    public class VideoBarServiceAtHome : VideoBarService
    {
        protected override string VIDEO_BAR_KEY => "VIDEO_BAR_KEY_AT_HOME";
    }
}