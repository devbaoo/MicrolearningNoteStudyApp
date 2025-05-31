using System.Collections.Generic;

namespace Common.Models
{
    public class SessionSettings
    {
        public int MaxAtoms { get; set; } = 20;
        public int TimeLimitMinutes { get; set; } = 30;
        public bool ShuffleOrder { get; set; } = true;
        public bool ShowHints { get; set; } = false;
    }
}
