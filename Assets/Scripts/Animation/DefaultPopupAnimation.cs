using System.Collections.Generic;
using CycladeUI.Popups.System;
using DG.Tweening;
using UnityEngine;

namespace CycladeUIExample.Animation
{
    public class DefaultPopupAnimation : BasePopupAnimation
    {
        [SerializeField] private List<DOTweenAnimation> animations;

        public override void SetupDefaultFromPopupSystem()
        {
            var o = transform.parent.gameObject;

            //todo: I don't know how to do it better... 
            animations[0].target = o.GetComponent<RectTransform>();
            animations[1].target = o.GetComponent<CanvasGroup>();
            
            animations[0].targetGO = animations[0].target.gameObject;
            animations[1].targetGO = animations[1].target.gameObject;

            gameObject.SetActive(true);
        }

        public override float PlayForward()
        {
            var maxDuration = float.MinValue;
            foreach (var anim in animations)
            {
                anim.DOPlayForward();
                if (maxDuration < anim.duration)
                    maxDuration = anim.duration;
            }
            return maxDuration;
        }

        public override float PlayBackward()
        {
            var maxDuration = float.MinValue;
            foreach (var anim in animations)
            {
                anim.DOPlayBackwards();
                if (maxDuration < anim.duration)
                    maxDuration = anim.duration;
            }
            return maxDuration;
        }
    }
}