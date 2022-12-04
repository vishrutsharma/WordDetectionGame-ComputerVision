using System;
using System.Collections.Generic;

namespace AR.Models
{
    [Serializable]
    public class DialogueData
    {
        public string name;
        public string dialogue;

    }

    [Serializable]
    public class DialogueModel
    {
        public List<DialogueData> dialogues;
    }
}


