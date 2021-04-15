using System;
using UnityEngine;
using UnityEngine.Events;

namespace Util
{
    public class OnCollisionEvents : MonoBehaviour
    {
        [SerializeField] private LayerMask layers;
        [SerializeField] private UnityEvent<Collider> onTriggerEnter;
        [SerializeField] private UnityEvent<Collider> onTriggerExit;
        [SerializeField] private UnityEvent<Collision> onCollisionEnter;
        [SerializeField] private UnityEvent<Collision> onCollisionExit;


        private void OnCollisionEnter(Collision other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onCollisionEnter.Invoke(other);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onCollisionExit.Invoke(other);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onTriggerEnter.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onTriggerExit.Invoke(other);
            }
        }
    }
}