using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Globalization;
using TwitchIntegration.Utils;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using UnityEngine;
using static TwitchIntegration.Connection;

namespace TwitchIntegration
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("dev.orangenote.twitchintegration", "TwitchIntegration", "1.1.0")]
    public class TwitchIntegration : BaseUnityPlugin
    {
        private static ConfigWrapper<bool> DisplayTwitchMessages { get; set; }

        private const string clientId = "vs0c496xny7uunkz41p8dvci1yf21e";

        private string accessToken;
        private string refreshToken;
        private static TwitchAPI api = new TwitchAPI();
        public static TwitchClient client = new TwitchClient();
        private static Connection twitchConnection = new Connection();
        private static string username;
        private static string channelName;

        public void Awake()
        {
            DisplayTwitchMessages = Config.Wrap(
                "Game",
                "DisplayTwitchMessages",
                "Enable or disable Twitch chat messages inside the game chat. Default is false (highly recommended, input is not sanitized).",
                false
            );

            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            twitchConnection.AuthorizeEventHandler += OnAuthorized;
        }

        [ConCommand(commandName = "twitch_connect", flags = ConVarFlags.None, helpText = "Connect to Twitch. Usage: twitch_connect [channel]")]
        [ConCommand(commandName = "tconnect", flags = ConVarFlags.None, helpText = "Alias for twitch_connect command. Usage: tconnect [channel]")]
        private static void OnTwitchConnectCommand(ConCommandArgs args)
        {
            Debug.Log("You will be redirected in a moment, please wait.");

            try
            {
                twitchConnection.Connect();
                channelName = args.Count > 0 ? args[0] : null;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.StackTrace);
            }
        }

        private async void OnAuthorized(object sender, AuthorizedEventArgs e)
        {
            this.accessToken = e.AccessToken;
            this.refreshToken = e.RefreshToken;

            api.Settings.ClientId = clientId;
            api.Settings.AccessToken = accessToken;

            Debug.Log("Authorized. Getting user info...");

            UserAuthed userAuthed = await api.V5.Users.GetUserAsync();

            Debug.Log(userAuthed.Name);

            username = userAuthed.Name;

            InitializeTwitchApi();
        }

        private void InitializeTwitchApi()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(username, this.accessToken);

            client.Initialize(credentials, channelName ?? username);

            client.OnMessageReceived += OnMessageReceived;
            client.OnConnected += (_, __) => Chat.AddMessage($"[<color=#6441a5>Twitch</color>] Connected to {channelName ?? username}'s channel as {username}");

            client.Connect();

            On.RoR2.Chat.AddMessage_ChatMessageBase += Chat_AddMessage_ChatMessageBase;
        }

        public static void SendChatMessage(string message)
        {
            if (client.IsConnected)
                client.SendMessage(channelName ?? username, message);
        }

        private void Chat_AddMessage_ChatMessageBase(On.RoR2.Chat.orig_AddMessage_ChatMessageBase orig, Chat.ChatMessageBase message)
        {
            if (message.GetType() == typeof(Chat.UserChatMessage))
            {
                var userMessage = (Chat.UserChatMessage)message;
                var text = userMessage.text;

                client.SendMessage(channelName ?? username, text);
            }

            orig(message);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!DisplayTwitchMessages.Value) return;

            string color = e.ChatMessage.ColorHex.IsNullOrWhiteSpace() ? "#e5eefc" : e.ChatMessage.ColorHex;

            var name = e.ChatMessage.DisplayName;

            string msg = string.Format(
                CultureInfo.InvariantCulture,
                "<color={0}>{1}</color>: {2}",
                color,
                Util.EscapeRichTextForTextMeshPro(name),
                Util.EscapeRichTextForTextMeshPro(e.ChatMessage.Message)
            );

            Chat.AddMessage(msg);
        }
    }
}