using System;
using UnityEngine;
using UnityEngine.Events;

namespace Util
{
    public class OnCollisionEvents2D : MonoBehaviour
    {
        [SerializeField] private LayerMask layers;
        [SerializeField] private UnityEvent<Collider2D> onTriggerEnter;
        [SerializeField] private UnityEvent<Collider2D> onTriggerExit;
        [SerializeField] private UnityEvent<Collision2D> onCollisionEnter;
        [SerializeField] private UnityEvent<Collision2D> onCollisionExit;


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onCollisionEnter.Invoke(other);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onCollisionExit.Invoke(other);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onTriggerEnter.Invoke(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (layers.Contains(other.gameObject.layer))
            {
                onTriggerExit.Invoke(other);
            }
        }
    }
}