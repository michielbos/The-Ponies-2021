using System.Collections;

using UnityEngine;

namespace UI.CAP
{
    /// <summary>
    /// Allows to move a highlighting background between at most three elements.
    /// </summary>
    public class HighlightSelectionMovingBackgroundController : MonoBehaviour
    {
        /// <summary>
        /// The speed of the moving background.
        /// </summary>
        [SerializeField]
        private float _movingSpeed = 1.0f;

        /// <summary>
        /// The moving background itself.
        /// </summary>
        [SerializeField]
        private Transform _movingBackground = null;

        /// <summary>
        /// The first immovable element.
        /// </summary>
        [SerializeField]
        private Transform _firstImmovable = null;

        /// <summary>
        /// The second immovable element.
        /// </summary>
        [SerializeField]
        private Transform _secondImmovable = null;

        /// <summary>
        /// The third immovable element
        /// </summary>
        [SerializeField]
        private Transform _thirdImmovable = null;

        /// <summary>
        /// The coroutine holder for the movement part.
        /// </summary>
        private Coroutine _movingBackgroundCoroutineHolder = null;

        /// <summary>
        /// Moves the background behind the first element.
        /// </summary>
        public void SetHighlightToFirstElement()
        {
            if (_movingBackgroundCoroutineHolder == null && _movingBackground != null && _firstImmovable != null)
                _movingBackgroundCoroutineHolder = StartCoroutine(MoveBackground(_firstImmovable.localPosition));
        }

        /// <summary>
        /// Mves the background behind the second element.
        /// </summary>
        public void SetHighlightToSecondElement()
        {
            if (_movingBackgroundCoroutineHolder == null &&  _movingBackground != null && _secondImmovable != null)
                _movingBackgroundCoroutineHolder = StartCoroutine(MoveBackground(_secondImmovable.localPosition));
        }

        /// <summary>
        /// Mves the background behind the third element.
        /// </summary>
        public void SetHighlightToThirdElement()
        {
            if (_movingBackgroundCoroutineHolder == null && _movingBackground != null && _thirdImmovable != null)
                _movingBackgroundCoroutineHolder = StartCoroutine(MoveBackground(_thirdImmovable.localPosition));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        private IEnumerator MoveBackground(Vector3 newPosition)
        {
            Vector3 differenceHolder = new Vector3();

            yield return new WaitUntil(() => 
                {
                    _movingBackground.localPosition = Vector3.Lerp(_movingBackground.localPosition, newPosition, _movingSpeed * Time.deltaTime);

                    differenceHolder = _movingBackground.localPosition - newPosition;
                    return Mathf.Abs(differenceHolder.x) <= 0.1f && Mathf.Abs(differenceHolder.y) <= 0.1f;
                });
            _movingBackgroundCoroutineHolder = null;
        }
    }
}
