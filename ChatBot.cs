using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace MaceBot
{
    internal class ChatBot
    {
        static string botName = "mace_bot";
        ConnectionCredentials creds = new ConnectionCredentials(botName, TwitchInfo.BotToken);
        TwitchClient client;
        string[] words = System.IO.File.ReadAllLines(@"C:\Users\Maceo\source\repos\MaceBot\BadWords.txt");

        internal void Connect()
        {
            client = new TwitchClient();
            client.Initialize(creds, TwitchInfo.ChannelName);

          //  client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnMessageSent += Client_OnMessageSent;
            client.OnRaidNotification += Client_OnRaidNotification;
         
            client.Connect();
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            client.SendMessage(e.Channel, $"Be sure to check out {e.RaidNotification.DisplayName} at http://www.twitch.tv/{e.RaidNotification.DisplayName}!");
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            switch (e.Command.CommandText.ToLower())
            {
                case "hibot":
                    client.SendMessage(TwitchInfo.ChannelName, "hello viewer");
                    break;

                case "roll":
                    RollDice();
                    break;
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine($"{e.ChatMessage.Channel}: {e.ChatMessage.Message}");
            foreach (string word in words)
            {
                if (e.ChatMessage.Message.Contains(word)) 
                {
                    client.DeleteMessage(e.ChatMessage.Channel, e.ChatMessage);
                    client.SendMessage(e.ChatMessage.Channel, "modCheck");
                }
            }
        }

        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            Console.WriteLine($"{botName}: {e.SentMessage.Message}");
        }

        private void RollDice()
        {
            var rand = new Random();
            client.SendMessage(TwitchInfo.ChannelName , rand.Next(7).ToString());
        }

        internal void Disconnect()
        {
            client.Disconnect();
        }
    }
}