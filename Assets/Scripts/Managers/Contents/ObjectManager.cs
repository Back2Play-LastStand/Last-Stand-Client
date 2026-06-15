using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayer MyPlayer { get; set; }
    Dictionary<ulong, GameObject> _objects = new Dictionary<ulong, GameObject>();

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        if (myPlayer)
        {
            // 이미 내 플레이어가 있으면 = 리스폰 또는 갱신
            if (_objects.TryGetValue(info.ObjectId, out GameObject existing))
            {
                MyPlayer my = existing.GetComponent<MyPlayer>();
                if (my != null)
                {
                    MyPlayer = my;
                    MyPlayer.Respawn(info);
                }

                return;
            }

            GameObject go = Managers.Resource.Instantiate("MyPlayer");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MyPlayer = go.GetComponent<MyPlayer>();
            MyPlayer.Id = info.ObjectId;
            MyPlayer.SetHealth(info.Health);
            MyPlayer.PosInfo = info.PosInfo;
            MyPlayer.UpdateHealthBar(MyPlayer);
            Managers.UI.m_Interface.SetTarget();
            Managers.UI.CloseAllPopupUI();
        }
        else
        {
            // 이미 있는 상대 플레이어면 새로 만들지 말고 갱신
            if (_objects.TryGetValue(info.ObjectId, out GameObject existing))
            {
                Player ohther = existing.GetComponent<Player>();
                if (ohther != null)
                {
                    ohther.SetHealth(info.Health);
                    ohther.PosInfo = info.PosInfo;
                    ohther.UpdateHealthBar(ohther);
                }

                return;
            }

            GameObject go = Managers.Resource.Instantiate("Player");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            Player player = go.GetComponent<Player>();
            player.Id = info.ObjectId;
            player.SetHealth(info.Health);
            player.PosInfo = info.PosInfo;
            player.UpdateHealthBar(player);
        }
    }
    public void AddMonster(ObjectInfo info)
    {
        GameObject go = Managers.Resource.Instantiate("Monster");
        //go.name = info.Name;
        go.name = $"Monster_{info.ObjectId}";
        _objects.Add(info.ObjectId, go);

        Monster monster = go.GetComponent<Monster>();
        monster.Id = info.ObjectId;
        monster.SetHealth(info.Health);
        monster.PosInfo = info.PosInfo;
        monster.UpdateHealthBar(monster);
    }
    public void Add(ulong id, GameObject go)
    {
        _objects.Add(id, go);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
            return;

        Remove(MyPlayer.Id);
        MyPlayer = null;
    }
    public void Remove(ulong id)
    {
        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);
        
        MyPlayer myPlayer = go.GetComponent<MyPlayer>();
        if (myPlayer != null && MyPlayer == myPlayer)
        {
            MyPlayer = null;
        }

        Managers.Resource.Destroy(go);
    }

    public GameObject FindById(ulong id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }
}
