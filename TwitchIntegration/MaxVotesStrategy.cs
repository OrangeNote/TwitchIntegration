using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TwitchIntegration
{
    class MaxVotesStrategy : IVoteStrategy
    {
        public Dictionary<string, string> voteResults { get; set; }
        public Dictionary<string, int> voteCounter { get; set; }

        public int Apply(List<string> options)
        {
            Debug.Log("start apply strategy");

            var votedValue = 0;
            var votedKey = options.ElementAt(0);

            foreach (var item in voteCounter)
            {
                if (item.Value > votedValue)
                {
                    votedKey = item.Key;
                    votedValue = item.Value;
                }
            }

            Debug.Log("votedKey: " + votedKey);

            return options.IndexOf(votedKey);
        }
    }
}
