using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        if (GameObject.Find("FollowPlayerCamera") == null)
            Managers.Resource.Instantiate("FollowPlayerCamera");
    }

    public override void Clear()
    {
    }
}
