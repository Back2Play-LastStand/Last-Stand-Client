using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    protected PlayerInput m_playerInput;
    Collider m_collider;
    public bool IsDead => m_isDead;
    bool m_isDead = false;

    protected override void Init()
    {
        base.Init();
        m_playerInput = GetComponent<PlayerInput>();
        m_collider = GetComponent<Collider>();

        // UI
        if (Managers.UI.m_Interface == null)
        {
            Managers.UI.m_Interface = Managers.UI.ShowSceneUI<UI_Interface>();
        }
        Managers.UI.m_Interface.SetTarget();
    }

    protected override void Update()
    {
        if (m_isDead) return;

        base.Update();

        UpdateAnim();
        UpdateRotation();

        if (m_playerInput.MouseClick)
            UpdateAttack();
        if (m_playerInput.ReloadButton)
            m_playerShoot.Reload();
    }

    MoveDir GetMoveDir(Vector2 moveVec)
    {
        if (moveVec == Vector2.zero)
            return MoveDir.None;

        float angle = Mathf.Atan2(moveVec.y, moveVec.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        if (angle >= 45f && angle < 135f) return MoveDir.Up;
        if (angle >= 135f && angle < 225f) return MoveDir.Left;
        if (angle >= 225f && angle < 315f) return MoveDir.Down;
        return MoveDir.Right;
    }

    protected override void UpdateMovement()
    {
        Vector2 vec = m_playerInput.InputVec;
        m_playerMovement.Move(vec);

        if (m_playerInput.InputVec != Vector2.zero)
        {
            vec = vec * m_playerMovement.m_speed * Time.deltaTime;
            PosInfo.PosX += vec.x;
            PosInfo.PosY += vec.y;

            Dir = GetMoveDir(m_playerInput.InputVec);
            REQ_MOVE move = new();
            move.Info = PosInfo;
            Managers.Network.Send(move, (ushort)PacketId.PKT_REQ_MOVE);
        }
        else
        {
            Dir = GetMoveDir(m_playerInput.InputVec);
            REQ_MOVE move = new();
            move.Info = PosInfo;
            Managers.Network.Send(move, (ushort)PacketId.PKT_REQ_MOVE);

        }
    }
    protected override void UpdateRotation()
    {
        Vector2 vec = m_playerInput.InputVec;
        if (vec != Vector2.zero)
        {
            Vector3 lookDir = new Vector3(vec.x, 0, vec.y);
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
    protected override void UpdateAttack()
    {
        base.UpdateAttack();
    }
    protected override void UpdateAnim()
    {
        Vector2 inputVec = m_playerInput.InputVec;

        if (inputVec == Vector2.zero)
        {
            m_playerMovement.PlayAnim(Vector2.zero);
            return;
        }

        Vector3 worldDir = new Vector3(inputVec.x, 0, inputVec.y);
        Vector3 localDir = transform.InverseTransformDirection(worldDir);
        Vector2 animDir = new Vector2(localDir.x, localDir.z);

        m_playerMovement.PlayAnim(animDir);
    }

    public override void Die()
    {
        Debug.Log($"{name} is dead");

        m_isDead = true;
        SetInput(false);
        SetCollider(false);
        SetVisible(false);

        Managers.UI.ShowPopupUI<UI_Respawn>();
    }

    public void Respawn(ObjectInfo info)
    {
        m_isDead = false;

        SetHealth(info.Health);
        PosInfo = info.PosInfo;
        TeleportTo(PosInfo, 4.0f);
        UpdateHealthBar(this);

        SetVisible(true);
        SetInput(true);
        SetCollider(true);

        Managers.UI.CloseAllPopupUI();
        Managers.UI.m_Interface.SetTarget();
    }

    public void SetInput(bool active)
    {
        if (m_playerInput != null)
            m_playerInput.enabled = active;
    }

    public void SetCollider(bool active)
    {
        if (m_collider != null)
            m_collider.enabled = active;
    }
    public void SetVisible(bool active)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
            renderer.enabled = active;
    }

    void OnApplicationQuit()
    {
        Managers.Network.ApplicationQuit();
    }
}
