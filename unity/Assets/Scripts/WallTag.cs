using System.Collections.Generic;
using UnityEngine;

namespace Dopeboyz
{
    public enum WallOwner
    {
        None,
        Crew,
        Rival
    }

    public class WallTag : MonoBehaviour
    {
        [Header("Spot Configuration")]
        public string spotName = "CANAL CORNER";
        public int spotIndex = 0;
        public int spotSize = 1; // 1 to 3
        public WallOwner owner = WallOwner.None;

        [Header("Spray Progress")]
        [Range(0f, 1f)]
        public float crewProgress = 0f;
        [Range(0f, 1f)]
        public float rivalProgress = 0f;
        public float sprayRate = 0.35f;

        [Header("Visual Colors")]
        public Color crewColor = new Color(0.298f, 0.961f, 0.835f); // Neon Turquoise
        public Color rivalColor = new Color(1.0f, 0.231f, 0.467f);   // Neon Red/Magenta
        public Color neutralColor = new Color(1.0f, 0.878f, 0.427f); // Gold

        [Header("Visual Layers")]
        public Renderer graffitiRenderer;
        public SpriteRenderer spriteGraffitiRenderer;
        public LineRenderer progressLine;
        public Transform crownBadge;

        private void Start()
        {
            UpdateVisuals();
        }

        public bool ApplySpray(string sprayer, float amount)
        {
            if (sprayer == "crew")
            {
                if (owner == WallOwner.Crew) return false;

                // Push back rival progress if rival owns it
                if (rivalProgress > 0f)
                {
                    rivalProgress -= amount * 1.5f;
                    if (rivalProgress <= 0f)
                    {
                        rivalProgress = 0f;
                        owner = WallOwner.None;
                    }
                }
                else
                {
                    crewProgress += amount;
                    if (crewProgress >= 1f)
                    {
                        crewProgress = 1f;
                        owner = WallOwner.Crew;
                        GameManager.Instance?.RegisterWallClaim("crew", spotName, spotSize);
                    }
                }
            }
            else if (sprayer == "rival")
            {
                if (owner == WallOwner.Rival) return false;

                if (crewProgress > 0f)
                {
                    crewProgress -= amount * 1.2f;
                    if (crewProgress <= 0f)
                    {
                        crewProgress = 0f;
                        owner = WallOwner.None;
                    }
                }
                else
                {
                    rivalProgress += amount;
                    if (rivalProgress >= 1f)
                    {
                        rivalProgress = 1f;
                        owner = WallOwner.Rival;
                        GameManager.Instance?.RegisterWallClaim("rival", spotName, spotSize);
                    }
                }
            }

            UpdateVisuals();
            return true;
        }

        public void ApplyInstantStencil(string sprayer)
        {
            if ((sprayer == "crew" && owner == WallOwner.Crew) || (sprayer == "rival" && owner == WallOwner.Rival)) return;
            if (sprayer == "crew")
            {
                crewProgress = 1f;
                rivalProgress = 0f;
                owner = WallOwner.Crew;
                GameManager.Instance?.RegisterWallClaim("crew", spotName, spotSize);
            }
            else
            {
                rivalProgress = 1f;
                crewProgress = 0f;
                owner = WallOwner.Rival;
                GameManager.Instance?.RegisterWallClaim("rival", spotName, spotSize);
            }
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            Color targetCol = neutralColor;
            float alpha = 0.4f;

            if (owner == WallOwner.Crew)
            {
                targetCol = crewColor;
                alpha = 1.0f;
            }
            else if (owner == WallOwner.Rival)
            {
                targetCol = rivalColor;
                alpha = 1.0f;
            }
            else
            {
                if (crewProgress > 0f)
                {
                    targetCol = crewColor;
                    alpha = 0.35f + crewProgress * 0.65f;
                }
                else if (rivalProgress > 0f)
                {
                    targetCol = rivalColor;
                    alpha = 0.35f + rivalProgress * 0.65f;
                }
            }

            // Update 3D MeshRenderer material if present
            if (graffitiRenderer != null && graffitiRenderer.material != null)
            {
                Color col = targetCol;
                col.a = alpha;
                graffitiRenderer.material.color = col;
            }

            // Update SpriteRenderer if present
            if (spriteGraffitiRenderer != null)
            {
                Color col = targetCol;
                col.a = alpha;
                spriteGraffitiRenderer.color = col;
            }

            if (crownBadge != null)
            {
                crownBadge.gameObject.SetActive(owner == WallOwner.Crew);
            }
        }
    }
}
