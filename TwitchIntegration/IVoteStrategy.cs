using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchIntegration
{
    public interface IVoteStrategy
    {
        int Apply(List<string> options);
        Dictionary<string, string> voteResults { get; set; }
        Dictionary<string, int> voteCounter { get; set; }
    }
}
