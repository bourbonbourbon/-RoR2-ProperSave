using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProperSave.Data
{
    public class DevotedLemurianData
    {
        [DataMember(Name = "ii")]
        public int itemIndex;

        [DataMember(Name = "dev")]
        public int devotedEvolutionLevel;

        public DevotedLemurianData(DevotedLemurianController controller)
        {
            itemIndex = (int)controller.DevotionItem;
            devotedEvolutionLevel = controller.DevotedEvolutionLevel;
        }

        public void LoadData(DevotedLemurianController controller)
        {
            controller._devotionItem = (RoR2.ItemIndex)itemIndex;
            controller._devotedEvolutionLevel = devotedEvolutionLevel;
        }
    }
}
