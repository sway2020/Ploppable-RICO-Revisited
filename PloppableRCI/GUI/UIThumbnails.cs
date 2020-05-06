using System.Linq;
using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// This class generates building preview thumbnails for the Ploppable Tool panel.
    /// Inspired by Boogieman Sam's FindIt! UI.
    /// </summary>
    public class Thumbnails
    {
        // Renderer for thumbnail images.
        private static UIPreviewRenderer thumbnailRenderer;


        /// <summary>
        /// Generates building thumbnail images (normal, focused, hovered, pressed and disabled) for the given building prefab.
        /// Thumbnails are applied to the m_Thumbnail and m_Atlas fields of the prefab.
        /// </summary>
        /// <param name="prefab">The BuildingInfo prefab to generate thumbnails for</param>
        /// <param name="name">The display name of the prefab.</param>
        public static void CreateThumbnail(BuildingData building)
        {
            // Create the renderer if it hasn't already been set up.
            if (thumbnailRenderer == null)
            {
                // Use a unique GameObject name to help find it with ModTools.
                thumbnailRenderer = new GameObject("RICORevisitedThumbnailRenderer").AddComponent<UIPreviewRenderer>();

                // Size and setting for thumbnail images: 109 x 100, doubled for anti-aliasing.
                thumbnailRenderer.Size = new Vector2(109, 100) * 2f;
                thumbnailRenderer.CameraRotation = 210f;
            }

            // Reset zoom.
            thumbnailRenderer.Zoom = 4f;

            // Don't do anything with null prefabs.
            if (building == null)
            {
                return;
            }

            // Set mesh and material for render from prefab.
            thumbnailRenderer.Mesh = building.prefab.m_mesh;
            thumbnailRenderer.material = building.prefab.m_material;

            if (thumbnailRenderer.Mesh == null)
            {
                // If the prefab itself has no mesh, see if there's any sub-buildings to render instead (e.g. Boston Residence Garage).
                if (building.prefab.m_subBuildings.Count() > 0)
                {
                    // Use first sub-building as render target; set mesh and material.
                    thumbnailRenderer.Mesh = building.prefab.m_subBuildings[0].m_buildingInfo.m_mesh;
                    thumbnailRenderer.material = building.prefab.m_subBuildings[0].m_buildingInfo.m_material;
                }
            }

            // If we still haven't gotten a mesh after the above, then something's not right; exit.
            if (thumbnailRenderer.Mesh == null)
            {
                Debug.Log("RICO Revisited: no thumbnail generated for null mesh " + building.prefab.name);
                return;
            }

            // If the selected building has colour variations, temporarily set the colour to the default for rendering.
            if (building.prefab.m_useColorVariations)
            {
                Color originalColor = building.prefab.m_material.color;
                building.prefab.m_material.color = building.prefab.m_color0;
                thumbnailRenderer.Render();
                building.prefab.m_material.color = originalColor;
            }
            else
            {
                // No temporary colour change needed.
                thumbnailRenderer.Render();
            }

            // Back up game's current active texture.
            RenderTexture gameActiveTexture = RenderTexture.active;

            // Convert the render to a 2D texture.
            Texture2D thumbnailTexture = new Texture2D(thumbnailRenderer.Texture.width, thumbnailRenderer.Texture.height);
            RenderTexture.active = thumbnailRenderer.Texture;
            thumbnailTexture.ReadPixels(new Rect(0f, 0f, (float)thumbnailRenderer.Texture.width, (float)thumbnailRenderer.Texture.height), 0, 0);
            thumbnailTexture.Apply();

            // Temporary texture for resizing render to thumbnail size (109 x 100).
            RenderTexture resizingTexture = RenderTexture.GetTemporary(109, 100);

            // Resize 2D texture (to 109 x 100) using trilinear filtering.
            resizingTexture.filterMode = FilterMode.Trilinear;
            thumbnailTexture.filterMode = FilterMode.Trilinear;

            // Resize.
            Graphics.Blit(thumbnailTexture, resizingTexture);
            thumbnailTexture.Resize(109, 100);
            thumbnailTexture.ReadPixels(new Rect(0, 0, 109, 100), 0, 0);
            thumbnailTexture.Apply();

            // Release temporary texture.
            RenderTexture.ReleaseTemporary(resizingTexture);

            // Restore game's current active texture.
            RenderTexture.active = gameActiveTexture;

            thumbnailTexture.name = building.displayName;
            building.prefab.m_Thumbnail = building.displayName;

            // Add new texture and thumbnail to prefab texture atlas.
            building.prefab.m_Atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            building.prefab.m_Atlas.name = "RICOThumbnails_" + building.displayName;
            building.prefab.m_Atlas.material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            building.prefab.m_Atlas.material.mainTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            AddTexturesToAtlas(building.prefab.m_Atlas, GenerateThumbnailVariants(thumbnailTexture));
        }


        /// <summary>
        /// Generates building thumbnail variants - focused, hovered, pressed, and disabled., 
        /// </summary>
        /// <param name="baseTexture">Base texture of the thumbnail</param>
        /// <returns>2d variant icon textures</returns>
        public static Texture2D[] GenerateThumbnailVariants(Texture2D baseTexture)
        {
            var variantPixels = new Color32[baseTexture.width * baseTexture.height];
            var basePixels = baseTexture.GetPixels32();


            // Focused.
            ColorFilter(basePixels, variantPixels, 32, 64, 128, 2);
            Texture2D focusedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            focusedTexture.SetPixels32(variantPixels);
            focusedTexture.Apply(false);
            focusedTexture.name = baseTexture.name + "Focused";

            // Hovered.
            ColorFilter(basePixels, variantPixels, 128, 128, 128, 1);
            Texture2D hoveredTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            hoveredTexture.SetPixels32(variantPixels);
            hoveredTexture.Apply(false);
            hoveredTexture.name = baseTexture.name + "Hovered";

            // Pressed.
            ColorFilter(basePixels, variantPixels, 128, 128, 128, 2);
            Texture2D pressedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            pressedTexture.SetPixels32(variantPixels);
            pressedTexture.Apply(false);
            pressedTexture.name = baseTexture.name + "Pressed";

            // Disabled.
            ColorFilter(basePixels, variantPixels, 0, 0, 0, 4);
            Texture2D disabledTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            disabledTexture.SetPixels32(variantPixels);
            disabledTexture.Apply(false);
            disabledTexture.name = baseTexture.name + "Disabled";

            return new Texture2D[]
            {
                baseTexture,
                focusedTexture,
                hoveredTexture,
                pressedTexture,
                disabledTexture
            };
        }


        /// <summary>
        /// Applies an RGB filter to a source colour, optionally reducing the intensity of the source colour before filtering (alpha is left unchanged).
        /// </summary>
        /// <param name="sourceColor">Source colour to filter</param>
        /// <param name="resultColor">Result of filtering</param>
        /// <param name="filterR">Red component of filter</param>
        /// <param name="filterG">Green component of filter</param>
        /// <param name="filterB">Blue component of filter</param>
        /// <param name="filterStrength">Each channel (RGB) of the original colour is bitshifted right this number before filtering (to reduce its intensity)</param>
        private static void ColorFilter(Color32[] sourceColor, Color32[] resultColor, byte filterR, byte filterG, byte filterB , byte filterStrength)
        {
            for (int i = 0; i < sourceColor.Length; i++)
            {
                // Rightshift the source channel by the required amount before adding the relevant filter channel.
                resultColor[i].r = (byte)((sourceColor[i].r >> filterStrength) + filterR);
                resultColor[i].g = (byte)((sourceColor[i].g >> filterStrength) + filterG);
                resultColor[i].b = (byte)((sourceColor[i].b >> filterStrength) + filterB);
                resultColor[i].a = sourceColor[i].a;
            }
        }
        

        /// <summary>
        /// Adds a collection of textures to an atlas.
        /// </summary>
        /// <param name="atlas">Atlas to add to</param>
        /// <param name="newTextures">Textures to add</param>
        public static void AddTexturesToAtlas(UITextureAtlas atlas, Texture2D[] newTextures)
        {
            Texture2D[] textures = new Texture2D[atlas.count + newTextures.Length];


            // Populate textures with sprites from the atlas.
            for (int i = 0; i < atlas.count; i++)
            {
                textures[i] = atlas.sprites[i].texture;
                textures[i].name = atlas.sprites[i].name;
            }

            // Append new textures to our list.
            for (int i = 0; i < newTextures.Length; i++)
            {
                textures[atlas.count + i] = newTextures[i];
            }
            
            // Repack atlas with our new additions (regions are individual texture areas within the atlas).
            Rect[] regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            // Clear original atlas sprites.
            atlas.sprites.Clear();

            // Iterate through our list, adding each sprite into the atlas.
            for (int i = 0; i < textures.Length; i++)
            {
                UITextureAtlas.SpriteInfo spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            // Rebuild atlas indexes.
            atlas.RebuildIndexes();
        }
    }
}