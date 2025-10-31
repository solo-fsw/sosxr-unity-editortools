using SOSXR.EnhancedLogger;
using UnityEngine;


namespace SOSXR
{
    public class SetHideFlags : MonoBehaviour
    {
        [SerializeField] private HideFlags m_hideFlags;
        [SerializeField] private Object[] m_objects;


        [ContextMenu(nameof(SetFlags))]
        public void SetFlags()
        {
            foreach (var obj in m_objects)
            {
                obj.hideFlags = m_hideFlags;

                this.Debug($"Hide-flags of {obj.name} is now {obj.hideFlags}");
            }
        }
    }
}