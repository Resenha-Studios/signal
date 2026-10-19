using Resenha.Signal.Core;
using Resenha.Signal.Data;
using UnityEngine;

namespace Resenha.Signal.Components
{
    [RequireComponent(typeof(Collider))]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, "SignalAudioManagerUnity.Components", "SignalAudioManagerUnity")]
    public class AmbientTrigger : MonoBehaviour
    {
        [Header("Trigger Configuration")]
        [Tooltip("The ambient type to transition to when entering this trigger.")]
        [SerializeField] private AmbientTypeSO ambientType = null;

        [Tooltip("Should this trigger activate when the player enter?")]
        [SerializeField] private bool triggerOnEnter = false;

        [Tooltip("Should this trigger activate when the player exits?")]
        [SerializeField] private bool triggerOnExit = false;

        [Tooltip("The ambient type to transition to upon exiting. Assign null for silence.")]
        [SerializeField] private AmbientTypeSO exitAmbientType = null;

        [Header("Transition Settings (Optional)")]
        [Tooltip("Override the fade-in time defined in the SO. Leave as -1 to use SO default.")]
        [SerializeField] private float overrideFadeIn = -1f;

        [Tooltip("Override the fade-out time defined in the SO. Leave as -1 to use SO default.")]
        [SerializeField] private float overrideFadeOut = -1f;

        [Header("Editor Visualization")]
        [Tooltip("Color of the trigger zone in the Scene View")]
        [SerializeField] private Color gizmoColor = new Color(0.2f, 0.8f, 0.4f, 0.3f);

        [Header("Player Settings")]
        [SerializeField] private string playerTag = "Player";

        private void Start()
        {
            var col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnEnter && other.CompareTag(playerTag))
            {
                AmbientSoundManager.Instance.ChangeAmbient(ambientType, false, overrideFadeOut, overrideFadeIn);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggerOnExit && other.CompareTag(playerTag))
            {
                AmbientSoundManager.Instance.ChangeAmbient(exitAmbientType, false, overrideFadeOut, overrideFadeIn);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var col = GetComponent<Collider>();
            if (col == null) return;

            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(sphere.center, sphere.radius);
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
#endif
    }
}