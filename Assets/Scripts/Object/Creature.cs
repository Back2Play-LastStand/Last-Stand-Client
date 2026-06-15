using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

public class Creature : HealthComponent, IDamage
{
    public uint Amount => _damage;
    [field: SerializeField]
    public uint _damage = 10;
    [field: SerializeField]
    public ulong Id { get; set; }
    
    const float TILE_SIZE = 0.1f;

    PositionInfo _positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            _positionInfo = value;
            _destPos = new Vector3(value.PosX * TILE_SIZE, transform.position.y, value.PosY * TILE_SIZE);
            transform.position = _destPos;
        }
    }
    public void TeleportTo(PositionInfo posInfo, float y)
    {
        _positionInfo = posInfo;

        _destPos = new Vector3(
            posInfo.PosX * TILE_SIZE,
            y,
            posInfo.PosY * TILE_SIZE
        );

        transform.position = _destPos;
    }

    public Vector3 VectorPos
    {
        get { return new Vector3(PosInfo.PosX, transform.position.y, PosInfo.PosY); }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.z;
        }
    }

    protected Vector3 _destPos;
    private UI_HealthBar m_healthBar;

    protected override void Start()
    {
        base.Start();

        Init();
    }

    protected virtual void Init()
    {
        m_healthBar = Managers.UI.MakeWorldSpaceUI<UI_HealthBar>(transform);
    }

    public void UpdateHealthBar(Creature creature)
    {
        if (m_healthBar == null)
            return;
        float ratio = creature.Health / (float)creature.maxHealth;
        m_healthBar.SetHealthRatio(ratio);
    }
    public void OnDamaged(Creature creature)
    {
        // damaged effect
        UpdateHealthBar(creature);
    }
    public virtual void Die()
    {
        Debug.Log($"{name} is dead");
        Managers.Object.Remove(Id);
    }
}
