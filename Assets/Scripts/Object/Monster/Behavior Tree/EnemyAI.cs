using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Monster
{
    public int detectiveRange;
    public int attackRange;

    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    SelectorNode rootNode;
    SequenceNode attackSequence;
    SequenceNode detectiveSequence;
    ActionNode idleAction;
    ActionNode returnAction;
    Transform target;

    [SerializeField] Animator anim;
    public LayerMask detectionLayer;

    Vector3 originPos;

    protected override void Start()
    {
        base.Start();

        originPos = transform.position;
        anim = GetComponent<Animator>();

        attackSequence = new SequenceNode();
        attackSequence.Add(new ActionNode(CheckInAttackRange));
        attackSequence.Add(new ActionNode(Attack));

        detectiveSequence = new SequenceNode();
        detectiveSequence.Add(new ActionNode(CheckInDetectiveRange));
        detectiveSequence.Add(new ActionNode(TraceTarget));

        returnAction = new ActionNode(ReturnAction);
        idleAction = new ActionNode(IdleAction);

        rootNode = new SelectorNode();
        rootNode.Add(attackSequence);
        rootNode.Add(detectiveSequence);
        rootNode.Add(returnAction);
        rootNode.Add(idleAction);
    }

    void Update()
    {
        //rootNode.Evaluate();
    }

    INode.STATE Attack()
    {
        if (target.GetComponent<Player>().Health <= 0)
            return INode.STATE.FAILURE;

        if (Time.time < lastAttackTime + attackCooldown)
            return INode.STATE.RUNNING;

        Debug.Log("공격중");
        // Add Attack Animation
        anim.SetTrigger("Attack");
        Debug.Log($"Attacker: {Id}");
        Protocol.REQ_ATTACK_OBJECT attack = new()
        {
            Attacker = Id,
            ObjectId = target.GetComponent<Player>().Id,
            Damage = Amount
        };
        Managers.Network.Send(attack, (ushort)PacketId.PKT_REQ_ATTACK_OBJECT);

        lastAttackTime = Time.time;

        return INode.STATE.RUNNING;
    }
    INode.STATE CheckInAttackRange()
    {
        if (target == null)
            return INode.STATE.FAILURE;
        if (Vector3.Distance(transform.position, target.position) < attackRange)
        {
            Debug.Log("공격 범위 감지 됨");
            return INode.STATE.SUCCESS;
        }

        return INode.STATE.FAILURE;
    }

    INode.STATE TraceTarget()
    {
        if (Vector3.Distance(transform.position, target.position) >= 0.1f)
        {
            Debug.Log("Trace!!");
            transform.forward = (target.position - transform.position).normalized;
            transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
            return INode.STATE.RUNNING;
        }
        else
            return INode.STATE.FAILURE;
    }
    INode.STATE CheckInDetectiveRange()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, detectiveRange, detectionLayer);
        if (cols.Length > 0)
        {
            Debug.Log("Detective..");
            foreach (Collider col in cols)
            {
                if (col.GetComponent<Player>() != null)
                {
                    target = col.transform;
                    return INode.STATE.SUCCESS;
                }
            }
        }

        return INode.STATE.FAILURE;
    }

    INode.STATE IdleAction()
    {
        Debug.Log("Idle..");
        return INode.STATE.RUNNING;
    }
    INode.STATE ReturnAction()
    {
        if (Vector3.Distance(transform.position, originPos) >= 0.1f)
        {
            Debug.Log("Return..");
            transform.forward = (originPos - transform.position).normalized;
            transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
            return INode.STATE.RUNNING;
        }
        else
            return INode.STATE.FAILURE;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectiveRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
