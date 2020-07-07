using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;


namespace PloppableRICO
{
    /// <summary>
    /// Render a 3d image of a given mesh.
    /// </summary>
    public class UIPreviewRenderer : MonoBehaviour
    {
        private Camera renderCamera;
        private Mesh currentMesh;
        private Bounds currentBounds;
        private float currentRotation;
        private float currentZoom;
        private Material _material;

        private List<BuildingInfo.MeshInfo> subMeshes;


        /// <summary>
        /// Sets material to render.
        /// </summary>
        public Material Material { set => _material = value; }


        /// <summary>
        /// Initialise the new renderer object.
        /// </summary>
        public UIPreviewRenderer()
        {
            // Set up camera.
            renderCamera = new GameObject("Camera").AddComponent<Camera>();
            renderCamera.transform.SetParent(transform);
            renderCamera.targetTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            renderCamera.allowHDR = true;
            renderCamera.enabled = false;

            // Basic defaults.
            renderCamera.pixelRect = new Rect(0f, 0f, 512, 512);
            renderCamera.backgroundColor = new Color(0, 0, 0, 0);
            renderCamera.fieldOfView = 30f;
            renderCamera.nearClipPlane = 1f;
            renderCamera.farClipPlane = 1000f;
        }


        /// <summary>
        /// Image size.
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(renderCamera.targetTexture.width, renderCamera.targetTexture.height);

            set
            {
                if (Size != value)
                {
                    // New size; set camera output sizes accordingly.
                    renderCamera.targetTexture = new RenderTexture((int)value.x, (int)value.y, 24, RenderTextureFormat.ARGB32);
                    renderCamera.pixelRect = new Rect(0f, 0f, value.x, value.y);
                }
            }
        }

        /// <summary>
        /// Sets mesh and material from a BuildingInfo prefab.
        /// </summary>
        /// <param name="prefab">Prefab to render</param>
        public void SetTarget(BuildingInfo prefab)
        {
            Mesh = prefab.m_mesh;
            _material = prefab.m_material;

            if (subMeshes == null)
            {
                subMeshes = new List<BuildingInfo.MeshInfo>();
            }
            else
            {
                subMeshes.Clear();
            }

            if (prefab.m_subMeshes != null && prefab.m_subMeshes.Length > 0)
            {
                for (int i = 0; i < prefab.m_subMeshes.Length; i++)
                {
                    subMeshes.Add(prefab.m_subMeshes[i]);
                }
            }
        }


        /// <summary>
        /// Currently rendered mesh.
        /// </summary>
        public Mesh Mesh
        {
            get => currentMesh;

            set => currentMesh = value;
        }


        /// <summary>
        /// Current building texture.
        /// </summary>
        public RenderTexture Texture
        {
            get => renderCamera.targetTexture;
        }


        /// <summary>
        /// Preview camera rotation (degrees).
        /// </summary>
        public float CameraRotation
        {
            get { return currentRotation; }
            set { currentRotation = value % 360f; }
        }


        /// <summary>
        /// Zoom level.
        /// </summary>
        public float Zoom
        {
            get { return currentZoom; }
            set
            {
                currentZoom = Mathf.Clamp(value, 0.5f, 5f);
            }
        }


        /// <summary>
        /// Render the current mesh.
        /// </summary>
        /// <param name="isThumb">True if this is a thumbnail render, false otherwise</param>
        public void Render(bool isThumb)
        {
            if (currentMesh == null)
            {
                return;
            }

            // Set background - plain if this is a thumbnail and the 'skybox' option isn't selected.
            if (isThumb && ModSettings.thumbBacks != (byte)ModSettings.ThumbBackCats.skybox)
            {
                // Is a thumbnail - user plain-colour background.
                renderCamera.clearFlags = CameraClearFlags.Color;

                // Set dark sky-blue background colour if the default 'color' background is set
                if (ModSettings.thumbBacks == (byte)ModSettings.ThumbBackCats.color)
                {
                    renderCamera.backgroundColor = new Color32(33, 151, 199, 255);
                }
            }
            else
            {
                // Not a thumbnail - use skybox background.
                renderCamera.clearFlags = CameraClearFlags.Skybox;
            }

            // Back up current game InfoManager mode.
            InfoManager infoManager = Singleton<InfoManager>.instance;
            InfoManager.InfoMode currentMode = infoManager.CurrentMode;
            InfoManager.SubInfoMode currentSubMode = infoManager.CurrentSubMode; ;

            // Set current game InfoManager to default (don't want to render with an overlay mode).
            infoManager.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            infoManager.UpdateInfoMode();

            // Backup current exposure and sky tint.
            float gameExposure = DayNightProperties.instance.m_Exposure;
            Color gameSkyTint = DayNightProperties.instance.m_SkyTint;

            // Backup current game lighting.
            Light gameMainLight = RenderManager.instance.MainLight;

            // Set exposure and sky tint for render.
            DayNightProperties.instance.m_Exposure = 0.5f;
            DayNightProperties.instance.m_SkyTint = new Color(0, 0, 0);
            DayNightProperties.instance.Refresh();

            // Set up our render lighting settings.
            Light renderLight = DayNightProperties.instance.sunLightSource;

            RenderManager.instance.MainLight = renderLight;

            // Set model position and calculate our rendering matrix.
            // We render at +100 Y to avoid garbage left at 0,0 by certain shaders and renderers (and we only rotate around the Y axis so will never see the origin).
            Vector3 modelPosition = new Vector3(0f, 100f, 0f);
            Matrix4x4 matrix = Matrix4x4.TRS(modelPosition, Quaternion.identity, Vector3.one);

            // Add our main mesh.
            Graphics.DrawMesh(currentMesh, matrix, _material, 0, renderCamera, 0, null, true, true);

            // Reset the bounding box to be the smallest that can encapsulate all verticies of the new mesh.
            // That way the preview image is the largest size that fits cleanly inside the preview size.
            currentBounds = new Bounds(Vector3.zero, Vector3.zero);

            // Use separate verticies instance instead of accessing Mesh.vertices each time (which is slow).
            // >10x measured performance improvement by doing things this way instead.
            Vector3[] vertices = currentMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                if (vertices[i].y > -2)
                {
                    currentBounds.Encapsulate(vertices[i]);
                }
            }

            // Render submeshes, if any.
            if (subMeshes != null && subMeshes.Count > 0)
            {
                foreach (BuildingInfo.MeshInfo subMesh in subMeshes)
                {
                    // Just in case.
                    if (subMesh?.m_subInfo?.m_mesh != null && subMesh?.m_subInfo?.m_material != null)
                    {
                        // Recalculate our matrix based on our submesh position and add the mesh to the render.
                        matrix = Matrix4x4.TRS(subMesh.m_position + modelPosition, Quaternion.identity, Vector3.one);
                        Graphics.DrawMesh(subMesh.m_subInfo.m_mesh, matrix, subMesh.m_subInfo.m_material, 0, renderCamera, 0, null, true, true);

                        // Expand our bounds to encapsulate the submesh.
                        vertices = subMesh.m_subInfo.m_mesh.vertices;
                        for (int i = 0; i < vertices.Length; i++)
                        {
                            // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                            if (vertices[i].y + subMesh.m_position.y > -2)
                            {
                                currentBounds.Encapsulate(vertices[i] + subMesh.m_position);
                            }
                        }
                    }
                }
            }

            // Set zoom to encapsulate entire model.
            float magnitude = currentBounds.extents.magnitude;
            float clipExtent = (magnitude + 16f) * 1.5f;
            float clipCenter = magnitude * currentZoom;

            // Clip planes.
            renderCamera.nearClipPlane = Mathf.Max(clipCenter - clipExtent, 0.01f);
            renderCamera.farClipPlane = clipCenter + clipExtent;

            // Rotate our camera around the model according to our current rotation.
            renderCamera.transform.position = modelPosition + (new Vector3(0f, 0.5f, 1f) * clipCenter);
            renderCamera.transform.RotateAround(modelPosition, Vector3.up, currentRotation);

            // Aim camera at middle of bounds.
            renderCamera.transform.LookAt(currentBounds.center + modelPosition);

            // If game is currently in nighttime, enable sun and disable moon lighting.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = true;
                DayNightProperties.instance.moonLightSource.enabled = false;
            }

            // Light settings.
            renderLight.transform.eulerAngles = new Vector3(55f, currentRotation - 180f, 0);
            renderLight.intensity = 2f;
            renderLight.color = Color.white;

            // Render!
            renderCamera.RenderWithShader(_material.shader, "");

            // Restore game lighting.
            RenderManager.instance.MainLight = gameMainLight;

            // Reset to moon lighting if the game is currently in nighttime.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = false;
                DayNightProperties.instance.moonLightSource.enabled = true;
            }

            // Restore game exposure and sky tint.
            DayNightProperties.instance.m_Exposure = gameExposure;
            DayNightProperties.instance.m_SkyTint = gameSkyTint;

            // Restore game InfoManager mode.
            infoManager.SetCurrentMode(currentMode, currentSubMode);
            infoManager.UpdateInfoMode();
        }
    }
}