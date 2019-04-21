using System;
using System.Timers;
using UnityEngine;
using UnityEngine.Networking;
using BepInEx;

namespace TwitchIntegration
{
    class Connection
    {
        private string baseUrl = "https://risk-of-rain-2.herokuapp.com";

        private Timer pollTimer;

        private StatusData statusData;

        private bool isRunning;
        private string sid;

        public void Connect()
        {
            isRunning = false;

            if (pollTimer != null)
                pollTimer.Stop();

            GetStatusData();
        }

        public void Poll(object sender, ElapsedEventArgs e)
        {
            GetStatusData();
        }

        private void GetStatusData()
        {
            var statusWebRequest = UnityWebRequest.Get(baseUrl + (sid.IsNullOrWhiteSpace() ? "/status" : "/status?sid=" + sid));
            
            statusWebRequest.SendWebRequest().completed += _ =>
            {
                if (statusWebRequest.isNetworkError || statusWebRequest.isHttpError)
                {
                    Debug.Log("statusWebRequest Error: " + statusWebRequest.error);
                }
                else
                {
                    statusData = JsonUtility.FromJson<StatusData>(statusWebRequest.downloadHandler.text);

                    Debug.Log("sid: " + statusData.sid);
                    Debug.Log("isRunning: " + isRunning);
                    Debug.Log("accessToken: " + statusData.access_token);

                    if (!isRunning)
                    {
                        isRunning = true;

                        sid = statusData.sid;

                        var browser = new System.Diagnostics.Process()
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo($"{baseUrl}/connect/twitch?sid={statusData.sid}") { UseShellExecute = true }
                        };

                        browser.Start();

                        pollTimer = new Timer(5000);
                        pollTimer.Elapsed += Poll;
                        pollTimer.Start();
                    }
                    else if (!statusData.access_token.IsNullOrWhiteSpace())
                    {
                        pollTimer.Stop();

                        AuthorizedEventArgs args = new AuthorizedEventArgs
                        {
                            AccessToken = statusData.access_token,
                            RefreshToken = statusData.refresh_token
                        };

                        OnAuthorized(args);
                    }
                }
            };
            
        }

        private class StatusData
        {
            public string error;
            public string sid;
            public string access_token;
            public string refresh_token;
        }

        protected virtual void OnAuthorized(AuthorizedEventArgs e)
        {
            AuthorizeEventHandler?.Invoke(this, e);
        }

        public event EventHandler<AuthorizedEventArgs> AuthorizeEventHandler;

        public class AuthorizedEventArgs : EventArgs
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

    }

}
