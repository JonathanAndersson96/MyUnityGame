using System;
using System.Collections.Generic;
using System.Linq;

namespace MyUnityGame.Battle
{
    // The demons the player carries and fights with, in order.
    public class Party
    {
        public const int MaxSize = 5;

        private readonly List<Demon> members;

        public Party(IEnumerable<Demon> demons)
        {
            members = demons?.ToList() ?? throw new ArgumentNullException(nameof(demons));
            if (members.Count > MaxSize || members.Any(demon => demon == null))
            {
                throw new ArgumentException($"A party holds at most {MaxSize} demons, none of them missing.",
                    nameof(demons));
            }
        }

        public IReadOnlyList<Demon> Members => members;

        internal void Remove(Demon demon) => members.Remove(demon);
    }
}
