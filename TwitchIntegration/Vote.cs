using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace TwitchIntegration
{
    public class Vote
    {
        private List<string> options;
        private IVoteStrategy voteStrategy = new MaxVotesStrategy();
        private Timer timer;
        private bool hasStarted;
        private Dictionary<string, string> voteResult = new Dictionary<string, string>();

        public Vote(List<string> options, double duration = 20000)
        {
            this.options = options;
            this.options.ForEach(o => o.ToUpper());

            this.timer = new Timer(duration);
        }

        public Vote(List<string> options, IVoteStrategy strategy, double duration = 20000) : this(options, duration)
        {
            this.voteStrategy = strategy;
        }

        public bool HasStarted()
        {
            return hasStarted;
        }

        public bool HasEnded()
        {
            return hasStarted && !timer.Enabled;
        }

        internal void Start()
        {
            voteResult.Clear();

            TwitchIntegration.client.OnMessageReceived += Client_OnMessageReceived;

            timer.Elapsed += (_, __) =>
            {
                Debug.Log("timer has elapsed");

                TwitchIntegration.client.OnMessageReceived -= Client_OnMessageReceived;

                timer.Stop();

                var voteCounter = new Dictionary<string, int>();
                foreach (var item in voteResult)
                {
                    voteCounter.TryGetValue(item.Value, out var count);
                    voteCounter[item.Value] = count + 1;
                }

                Debug.Log("ready to send onend event");

                voteStrategy.voteResults = voteResult;
                voteStrategy.voteCounter = voteCounter;

                EndEventArgs args = new EndEventArgs
                {
                    VoteResult = voteResult,
                    VoteCounter = voteCounter,
                    VotedIndex = voteStrategy.Apply(options)
                };

                OnEnd(args);
            };

            timer.Start();
            hasStarted = true;
            OnStart();
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var msg = e.ChatMessage.Message.Trim().ToUpper();

            if (options.Contains(msg))
            {
                voteResult[e.ChatMessage.DisplayName] = msg;
            }
        }

        protected virtual void OnStart()
        {
            StartEventHandler?.Invoke(this, null);
        }

        protected virtual void OnEnd(EndEventArgs e)
        {
            EndEventHandler?.Invoke(this, e);
        }

        public event EventHandler<EndEventArgs> EndEventHandler;
        public event EventHandler StartEventHandler;

        public class EndEventArgs : EventArgs
        {
            public Dictionary<string, string> VoteResult { get; set; }
            public Dictionary<string, int> VoteCounter { get; set; }
            public int VotedIndex { get; set; }
        }
    }
}
