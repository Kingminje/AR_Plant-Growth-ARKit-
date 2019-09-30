using System.Collections;
using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// Class for creating interaction area to interact with target object
    /// Update tick called only when object is in trigger range
    /// </summary>
    public abstract class FBasic_InteractionAreaBase : MonoBehaviour
    {
        protected SphereCollider triggerArea;
        //V1.1.1
        public bool Entered { get; protected set; }

        protected virtual void Start()
        {
            Entered = false;
            GetTrigger();
        }

        protected virtual void OnValidate()
        {
            GetTrigger();
        }

        /// <summary>
        /// Returning sphere collider trigger, creates one if not exists already
        /// </summary>
        protected SphereCollider GetTrigger()
        {
            if (!triggerArea)
            {
                triggerArea = GetComponent<SphereCollider>();

                if (!triggerArea)
                {
                    triggerArea = gameObject.AddComponent<SphereCollider>();
                    triggerArea.radius = 1f;
                    triggerArea.isTrigger = true;

                    gameObject.layer = LayerMask.NameToLayer("IgnoreRaycast");
                }
            }

            return triggerArea;
        }

        /// <summary>
        /// Method called every frame when player is in trigger range
        /// </summary>
        protected virtual void UpdateIn()
        {

        }

        /// <summary>
        /// Coroutine called when player is in trigger range
        /// </summary>
        protected IEnumerator UpdateIfInRange()
        {
            while(true)
            {
                //yield return new WaitForEndOfFrame();
                UpdateIn();
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Entered) return;

            if ( other.tag == "Player")
            {
                StartCoroutine(UpdateIfInRange());
                OnEnter();
                Entered = true;
            }
        }

        protected virtual void OnEnter()
        {

        }

        private void OnTriggerExit(Collider other)
        {
            if (!Entered) return;

            if (other.tag == "Player")
            {
                StopAllCoroutines();
                OnExit();
                Entered = false;
            }
        }

        protected virtual void OnExit()
        {

        }
    }
}
