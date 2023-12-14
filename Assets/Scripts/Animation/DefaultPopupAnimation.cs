using System.Collections.Generic;
using CycladeUI.Popups.System;
using DG.Tweening;
using UnityEngine;

namespace CycladeUIExample.Animation
{
    public class DefaultPopupAnimation : BasePopupAnimation
    {
        [SerializeField] private List<DOTweenAnimation> animations;

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