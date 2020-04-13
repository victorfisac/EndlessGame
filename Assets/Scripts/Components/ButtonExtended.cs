using UnityEngine;
using UnityEngine.UI;


namespace EndlessGame.Components
{
    [RequireComponent(typeof(ButtonExtendedAnimations))]
    public class ButtonExtended : Button
    {
        private ButtonExtendedAnimations m_animatedButton = null;


        protected override void Awake()
        {
            base.Awake();

            m_animatedButton = GetComponent<ButtonExtendedAnimations>();
            DoStateTransition(currentSelectionState, true);
        }


        protected override void DoStateTransition(SelectionState pState, bool pInstant)
        {
            base.DoStateTransition(pState, pInstant);
            m_animatedButton.SetButtonState((int)pState);
        }
    }
}