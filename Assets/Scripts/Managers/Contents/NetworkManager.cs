using Google.Protobuf;
using Protocol;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using ServerCore;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();
    Connector _connector = new Connector();
    IPAddress _ipAddr;
    IPEndPoint _ipEndPoint;

    private Action<Session> _onConnected;
    private bool _connected;

    public void Init(int port)
    {
        _ipAddr = IPAddress.Parse("127.0.0.1");
        _ipEndPoint = new IPEndPoint(_ipAddr, port);
    }

    public void ConnectServer(Action<Session> success)
    {
        _onConnected = success;
        _connector.Connect(_ipEndPoint,
            () =>
            {
                _connected = true;
                return _session;
            });
    }

    public void Update()
    {
        if (_connected)
        {
            _connected = false;
            _onConnected?.Invoke(_session);
        }

        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = Managers.Packet.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }

    public void Send(IMessage packet, ushort id)
    {
        _session.Send(packet, id);
    }

    public void ApplicationQuit()
    {
        Logout();
        _connector.Disconnect();
    }

    public void Logout()
    {
        string sessionId = PlayerPrefs.GetString("SESSION_ID");

        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogWarning("세션 ID가 없습니다. 이미 로그아웃되었을 수 있습니다.");
            return;
        }

        string url = $"{WebRequestManager.Instance.serverConn.LogoutUrl}?sessionId={sessionId}";

        UnityWebRequest request = UnityWebRequest.Delete(url);
        var asyncOp = request.SendWebRequest();

        while (!asyncOp.isDone)
        {

        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("로그아웃 성공: " + request.downloadHandler.text);

            PlayerPrefs.DeleteKey("SESSION_ID");
            PlayerPrefs.DeleteKey("USER_ID");
        }
        else
        {
            Debug.LogError($"로그아웃 실패: {request.responseCode} - {request.downloadHandler.text}");
        }
    }
}