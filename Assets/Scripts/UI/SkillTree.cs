using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillTree 
{
    private SkillNode rootNode;
    public class SkillNode
    {
        public string name;
        private bool locked;
        private bool unlockable;
        private List<SkillNode> children;

        SkillNode(string name, List<string> childrenNames , bool locked=true, bool unlockable = false)
        {
            this.name = name;
            this.locked = locked;
            this.unlockable = unlockable;
            children = new List<SkillNode>(childrenNames.Count);
            for(int i = 0;i < childrenNames.Count;i++)
            {
                children[i].name = childrenNames[i];
            }
        }

        public bool IsLocked => locked;
        public bool Unlockable => unlockable;
        public void ApplyUpgrade(bool locked)
        {
            this.locked = locked;
            if (!locked)
            {
                foreach(SkillNode node in children)
                {
                    node.SetUnlockable(true);
                }
            }
        }

        private void SetUnlockable(bool unlockable)
        {
            this.unlockable = unlockable;
        }

    }

}
