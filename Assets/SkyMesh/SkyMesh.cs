using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Draw a user-defined mesh as sky.
/// The shader of the material should be SkyMesh shader.
/// Skybox is recommended if you use GI.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SkyMesh : MonoBehaviour
{
    [SerializeField] Mesh m_mesh;
    [SerializeField] Material m_material;
    [SerializeField] CameraEvent m_cameraEvent = CameraEvent.AfterImageEffectsOpaque;
    [SerializeField] Vector3 m_position = Vector3.zero;
    [SerializeField] Vector3 m_rotation = Vector3.zero;
    [SerializeField] Vector3 m_scale = Vector3.one;

    CameraEvent lastCameraEvent;
    CommandBuffer commandBuffer;

    public Mesh mesh
    {
        get { return m_mesh; }
        set
        {
            m_mesh = value;
            if (enabled) OnEnable();
        }
    }

    public Material material {
        get { return m_material; }
        set
        {
            m_material = value;
            if (enabled) OnEnable();
        }
    }

    public CameraEvent cameraEvent {
        get { return m_cameraEvent; }
        set
        {
            m_cameraEvent = value;
            if (enabled) OnEnable();
        }
    }

    public Vector3 position {
        get { return m_position; }
        set
        {
            m_position = value;
            if (enabled) OnEnable();
        }
    }

    public Vector3 rotation {
        get { return m_rotation; }
        set
        {
            m_rotation = value;
            if (enabled) OnEnable();
        }
    }

    public Vector3 scale {
        get { return m_scale; }
        set
        {
            m_scale = value;
            if (enabled) OnEnable();
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (enabled)
        {
            if (GetComponent<Camera>().clearFlags == CameraClearFlags.Skybox)
                Debug.LogWarning("SkyMesh: You should turn off Skybox.");

            OnEnable();
        }
    }
#endif

    void OnEnable()
    {
        OnDisable();
        var camera = GetComponent<Camera>();
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "SkyMesh";
        commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        var matrix = Matrix4x4.TRS(m_position, Quaternion.Euler(m_rotation), m_scale);
        commandBuffer.DrawMesh(m_mesh, matrix, m_material);
        camera.AddCommandBuffer(m_cameraEvent, commandBuffer);
        lastCameraEvent = m_cameraEvent;
    }

    void OnDisable()
    {
        if (commandBuffer == null)
            return;

        var camera = GetComponent<Camera>();
        camera.RemoveCommandBuffer(lastCameraEvent, commandBuffer);
        commandBuffer.Dispose();
        commandBuffer = null;
    }
}
