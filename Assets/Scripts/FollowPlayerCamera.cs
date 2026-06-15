using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    public CinemachineCamera virtualCamera;

    Transform _target;

    void Awake()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineCamera>();
    }

    void LateUpdate()
    {
        if (Managers.Object.MyPlayer == null)
            return;

        Transform player = Managers.Object.MyPlayer.transform;

        if (_target == player)
            return;

        _target = player;

        virtualCamera.Follow = _target;
        virtualCamera.LookAt = _target;
    }
}