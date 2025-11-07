using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface ICurrentMatchService
    {
        void setMatchId(string matchId);
        string getMatchId();
    }
}
