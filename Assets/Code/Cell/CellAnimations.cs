using DG.Tweening;
using UnityEngine;

namespace Code.Cell
{
    public static class CellAnimations
    {
        public static void DoTurnAnimation(this Transform transform, float target, float duration)
        {
            transform.DOScale(FindTargetScale(transform, target), duration)
                .From()
                .SetEase(Ease.OutElastic);
        }

        public static void DoPulse(this Transform transform, float duration, float target)
        {
            transform.DOScale(FindTargetScale(transform, target), duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public static void DoUIAppearEffect(this Transform transform)
        {
            transform.DOScale(Vector3.one * 0.6f, 1f)
                .From()
                .SetEase(Ease.OutElastic);
        }
        
        private static Vector3 FindTargetScale(Transform transform, float target)
        {
            var originScale = transform.localScale;
            var targetScale = originScale.Change(x: originScale.x * target, z: originScale.z * target);
            
            return targetScale;
        }

        private static Vector3 Change(this Vector3 target, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? target.x, y ?? target.y, z ?? target.z);
        }
    }
}