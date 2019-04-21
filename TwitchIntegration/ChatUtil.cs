using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;

namespace TwitchIntegration
{
    public class ChatUtil
    {
        private Dictionary<string, List<Func<string, string[], bool>>> chatCommandList = new Dictionary<string, List<Func<string, string[], bool>>>();

        public ChatUtil()
        {
            On.RoR2.Chat.AddMessage_string += (orig, message) =>
            {
                orig(message);
                if (ParseUserAndMessage(message, out string userName, out string text))
                {
                    string[] pieces = text.Split(' ');
                    TriggerChatCommand(userName, pieces);
                }
            };
        }

        public void SendChat(string message)
        {
            Chat.SendBroadcastChat(new Chat.PlayerChatMessage()
            {
                networkPlayerName = new NetworkPlayerName()
                {
                    steamId = new CSteamID(1234),
                    nameOverride = "user"
                },
                baseToken = message
            });
        }

        public void AddChatCommand(string command, Func<string, string[], bool> @delegate)
        {
            command = command.ToUpper();
            if (!chatCommandList.ContainsKey(command))
            {
                chatCommandList.Add(command, new List<Func<string, string[], bool>>());
            }
            if (chatCommandList.TryGetValue(command, out var funcList))
            {
                funcList.Add(@delegate);
            }
        }

        private bool ParseUserAndMessage(string input, out string user, out string message)
        {
            user = "";
            message = "";
            int ix = input.IndexOf("<noparse>/");
            if (ix >= 0)
            {
                int start = "<color=#123456><noparse>".Length;
                int len = ix - "</noparse>:0123456789012345678901234".Length; // lol
                user = input.Substring(start, len);
                message = input.Substring(ix + "<noparse>/".Length);
                message = message.Substring(0, message.IndexOf("</noparse>"));
                return true;
            }

            return false;
        }

        public void TriggerChatCommand(string userName, string[] pieces)
        {
            if (pieces.Length == 0)
            {
                return;
            }
            if (chatCommandList.TryGetValue(pieces[0].ToUpper(), out var list))
            {
                foreach (var func in list)
                {
                    try
                    {
                        func.Invoke(userName, pieces);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Command " + pieces[0] + " logged exception \"" + ex.Message + "\"");
                    }
                }
            }
        }
    }
}
