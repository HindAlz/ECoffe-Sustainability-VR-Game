using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityPeople
{
    public class CityPeople : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Autoplay random animation clips")]
        private bool AutoPlayAnimations = true;

        [SerializeField]
        [Tooltip("Overrides palette materials, skips other objects")]
        private Material PaletteOverride;

        [SerializeField]
        [Tooltip("List of possible palette materials for random selection")]
        private List<Material> PaletteOverridesList;

        public string CurrentPaletteName { get; private set; }

        private AnimationClip[] myClips;
        private Animator animator;
        public const string people_pal_prefix = "people_pal";
        private List<Renderer> _paletteMeshes;

        private void Awake()
        {
            var AllRenderers = gameObject.GetComponentsInChildren<Renderer>();
            _paletteMeshes = new List<Renderer>();
            animator = GetComponent<Animator>();

            foreach (Renderer r in AllRenderers)
            {
                var matName = r.sharedMaterial.name;
                var len = Math.Min(people_pal_prefix.Length, matName.Length);
                if (matName.Substring(0, len) == CityPeople.people_pal_prefix)
                {
                    _paletteMeshes.Add(r);
                }
            }

            if (_paletteMeshes.Count > 0)
            {
                CurrentPaletteName = _paletteMeshes[0].sharedMaterial.name;
            }

            if (PaletteOverridesList.Count > 0)
            {
                SetRandomPalette();
            }
            else if (PaletteOverride != null)
            {
                SetPalette(PaletteOverride);
            }
        }


        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetPalette(Material mat)
        {
            if (mat != null)
            {
                if (mat.name[0..people_pal_prefix.Length] == CityPeople.people_pal_prefix)
                {
                    CurrentPaletteName = mat.name;
                    foreach (Renderer r in _paletteMeshes)
                    {
                        r.material = mat;
                    }
                }
                else
                {
                    Debug.Log("Material name should start with 'people_pal...' by convention.");
                }
            }
        }

        // New method to set a random palette from the list
        public void SetRandomPalette()
        {
            if (PaletteOverridesList.Count > 0)
            {
                // Select a random material from the list
                int randomIndex = Random.Range(0, PaletteOverridesList.Count);
                SetPalette(PaletteOverridesList[randomIndex]);
            }
            else
            {
                Debug.LogWarning("No palette materials available in the list.");
            }
        }

        public void SetWalking(bool isWalking)
        {
            if (animator != null)
            {
                animator.SetBool("isWalking", isWalking);
                Debug.Log($"Setting walking to {isWalking}");
            }
            else
            {
                Debug.LogWarning("Animator not found in CityPeople.");
            }
        }
    }
}