using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "ServerConnection", menuName = "Configs/ServerConnection")]
public class ServerConnection : ScriptableObject
{
    public string BaseUrl = "http://127.0.0.1:5075/api";

    public string RegisterEndpoint = "/auth/register";
    public string LoginEndpoint = "/auth/login";
    public string VerifyCodeEndpoint = "/verify/send";
    public string VerifyEndpoint = "/verify";
    public string PostPlayerNameEndpoint = "/data/name";
    public string LogoutEndpoint = "/logout";

    public string RegisterUrl => BaseUrl + RegisterEndpoint;
    public string LoginUrl => BaseUrl + LoginEndpoint;
    public string VerifyCodeUrl => BaseUrl + VerifyEndpoint;
    public string PostPlayerNameUrl => BaseUrl + PostPlayerNameEndpoint;
    public string LogoutUrl => BaseUrl + LogoutEndpoint;

    public string GetVerifyEmailUrl(string email) => $"{BaseUrl}/verify/send?email={UnityWebRequest.EscapeURL(email)}";
    public string GetTopRankingEndpoint(int count) => $"/ranking/top/{count}";
    public string GetTopRankingUrl(int count) => BaseUrl + GetTopRankingEndpoint(count);
    public string GetPlayerRankingUrl(string playerId) => $"{BaseUrl}/ranking/{playerId}";
    public string GetPlayerNameUrl(string playerId) => $"{BaseUrl}/data/name?playerId={UnityWebRequest.EscapeURL(playerId)}";
}
