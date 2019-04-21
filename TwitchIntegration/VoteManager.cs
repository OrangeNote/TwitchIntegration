using System;
using System.Collections.Generic;

namespace TwitchIntegration
{
    public static class VoteManager
    {
        private static Queue<Vote> queue = new Queue<Vote>();

        public static void AddVote(Vote vote)
        {
            queue.Enqueue(vote);

            vote.EndEventHandler += (_, __) => StartNext();

            StartNext();
        }

        private static void StartNext()
        {
            try
            {
                var currentVote = queue.Peek();
                if (!currentVote.HasStarted())
                {
                    currentVote.Start();
                }
                else if (currentVote.HasEnded())
                {
                    queue.Dequeue();
                    StartNext();
                }
            }
            catch (InvalidOperationException) { }
        }
    }
}
