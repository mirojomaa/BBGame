using UnityEngine;
using UnityEngine.Rendering;
using NaughtyAttributes;
[ExecuteAlways]
public class BenderManager : MonoBehaviour
{
  private const string BENDER = "ENABLE_BENDING", PLANET = "ENABLE_BENDING_PLANET";
  private static readonly int BENDINGAMOUNT = Shader.PropertyToID("_BendingAmount");
  private bool enablePlanet = true;
  private float _prevAmount;
  private static float cullingMatrixHor , cullingMatrixVert;
  [InfoBox("Don't touch the Bending Amount if you don't know what you are doing. Default is 0.00988", EInfoBoxType.Warning)]
  [Tooltip("Changes how much the Planet gets Bend. It is in propotion to the Vert distance from the cameras")]
  [BoxGroup("Bending")] [SerializeField] [Range(-0.1f, 0.1f)] private float bendingAmount = 0.00988f;
  [InfoBox("You can change the Frustum Culling Settings here. Do this if you experience Object popping. \n " +
               "It affects performance greatly by keeping the Frustum Cull tight. Default is 8 | 2 | 4")]
  [BoxGroup("Frustum Culling")] [Tooltip("Frustum Culls less (higher number) or more (lower number) dependend on the FoV as basenumber")]
  [Range(-67, 127)] [SerializeField] private float DrawDistance = 8;
  [BoxGroup("Frustum Culling")] [Tooltip("Adds or substracts a certain amount in the Horizontal Axis compared to the Vertical Axis")]
  [Range(-32, 31)] [SerializeField] private float HorizontalMod = 2;
  [BoxGroup("Frustum Culling")] [Tooltip("Takes the distance to the ground and multiplys it by this number. Higher number means less culling when in air")]
  [Range(0, 10)] [SerializeField] private float HeightMod = 4;
  private void Awake ()
  {
    if ( Application.isPlaying ) Shader.EnableKeyword(BENDER);
    else Shader.DisableKeyword(BENDER);
    if ( enablePlanet ) Shader.EnableKeyword(PLANET);
    else Shader.DisableKeyword(PLANET);
    UpdateBendingAmount();
    if (Application.isPlaying)
    {
      InvokeRepeating("UpdateBendingAmount", 0.1f, 0.5f);
      InvokeRepeating("UpdateCullingMatrix", 0f, 0.15f);
    }

  }
  private void OnEnable ()
  {
    if ( !Application.isPlaying ) return;
    RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
  }
  private void OnDisable ()  //SceneView
  {
    RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
  }
  private void UpdateBendingAmount () => Shader.SetGlobalFloat(BENDINGAMOUNT, bendingAmount);

  private static void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam) =>
    cam.cullingMatrix = Matrix4x4.Ortho(-cullingMatrixHor, cullingMatrixHor, -cullingMatrixVert, cullingMatrixVert, 0.0001f, 3*cullingMatrixVert) *
                        cam.worldToCameraMatrix;
  private void UpdateCullingMatrix()
  {
    if (CameraZoomOut.vcamera.m_Lens.FieldOfView < 110)
    {
      cullingMatrixVert = DrawDistance + PlayerMovement.distanceToGround*HeightMod + CameraZoomOut.vcamera.m_Lens.FieldOfView;
      cullingMatrixHor = HorizontalMod + cullingMatrixVert;
    }
    else if (CameraZoomOut.vcamera.m_Lens.FieldOfView >= 110)
    {
      cullingMatrixVert= PlayerMovement.distanceToGround*HeightMod + DynamiclyScaleCulling(CameraZoomOut.vcamera);
      cullingMatrixHor = HorizontalMod + cullingMatrixVert * 1.15f;
    }
  }
  private static float DynamiclyScaleCulling(Cinemachine.CinemachineVirtualCamera vcam) =>
    MathLibary.RemapClamped(110, 180, 140, 260, vcam.m_Lens.FieldOfView);
  private static void OnEndCameraRendering (ScriptableRenderContext ctx, Camera cam) => cam.ResetCullingMatrix();
}