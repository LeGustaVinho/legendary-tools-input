using UnityEngine;

namespace LegendaryTools.Input
{
    public delegate void On3DWorldHitEventHandler(RaycastHit HitInfo);

    public delegate void On2DWorldHitEventHandler(RaycastHit2D HitInfo);

    [RequireComponent(typeof(Camera))]
    public class ScreenToWorldInfo : MonoBehaviour
    {
        public enum EventTriggerType
        {
            PointerMove,
            PointerUp,
            PointerDown
        }

        public enum Mode
        {
            Physics3D,
            Physics2D
        }

        public Camera Camera;

        public bool CanInput = true;

        public LayerMask CullingMask;

        public EventTriggerType EventTrigger;


        public bool HasSomething;

        [HideInInspector] public RaycastHit HitInfo;

        [HideInInspector] public RaycastHit2D HitInfo2D;

        public Mode RayCastMode;

        public bool ShowDebug = true;
        public KeyCode TriggerKey = KeyCode.Mouse0;

        public Transform Transform => RayCastMode == Mode.Physics3D ? HitInfo.transform : HitInfo2D.transform;

        public float Distance => RayCastMode == Mode.Physics3D ? HitInfo.distance : HitInfo2D.distance;

        public Vector3 Position => RayCastMode == Mode.Physics3D ? HitInfo.point : (Vector3) HitInfo2D.point;

        public Vector3 Normal => RayCastMode == Mode.Physics3D ? HitInfo.normal : (Vector3) HitInfo2D.normal;

        public Collider Collider => HitInfo.collider;

        public Vector2 TextureCoords => HitInfo.textureCoord;

        public Vector2 TextureCoords2 => HitInfo.textureCoord2;

        public Rigidbody Rigidbody => HitInfo.rigidbody;

        public int TriangleIndex => HitInfo.triangleIndex;

        public Vector2 Centroid => HitInfo2D.centroid;

        public Collider2D Collider2D => HitInfo2D.collider;

        public float Fraction => HitInfo2D.fraction;

        public Rigidbody2D Rigidbody2D => HitInfo2D.rigidbody;

        public event On3DWorldHitEventHandler On3DHit;
        public event On2DWorldHitEventHandler On2DHit;

        // Update is called once per frame
        private void Update()
        {
            if (CanInput)
            {
                switch (EventTrigger)
                {
                    case EventTriggerType.PointerMove:
#if UNITY_ANDROID || UNITY_IPHONE
                        foreach (Touch touch in UnityEngine.Input.touches)
                            LauchRay(touch.position);
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
                        lauchRay(UnityEngine.Input.mousePosition);
#endif
                        break;
                    case EventTriggerType.PointerUp:

#if UNITY_ANDROID || UNITY_IPHONE
                        foreach (Touch touch in UnityEngine.Input.touches)
                        {
                            if(touch.phase == TouchPhase.Began)
                                LauchRay(touch.position);
                        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
                        if (UnityEngine.Input.GetKeyUp(TriggerKey))
                        {
                            lauchRay(UnityEngine.Input.mousePosition);
                        }
#endif
                        break;
                    case EventTriggerType.PointerDown:
#if UNITY_ANDROID || UNITY_IPHONE
                        foreach (Touch touch in UnityEngine.Input.touches)
                        {
                            if (touch.phase == TouchPhase.Ended)
                                LauchRay(touch.position);
                        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
                        if (UnityEngine.Input.GetKeyDown(TriggerKey))
                        {
                            lauchRay(UnityEngine.Input.mousePosition);
                        }
#endif
                        break;
                }
            }
        }

        private void lauchRay(Vector3 position)
        {
            Ray ray = Camera.ScreenPointToRay(position);

            if (ShowDebug)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);
            }

            if (RayCastMode == Mode.Physics3D)
            {
                if (Physics.Raycast(ray, out HitInfo, Mathf.Infinity, CullingMask.value))
                {
                    if (ShowDebug)
                    {
                        Debug.DrawLine(transform.position, HitInfo.point, Color.red, 1);
                    }

                    HasSomething = true;

                    if (On3DHit != null)
                    {
                        On3DHit.Invoke(HitInfo);
                    }
                }
                else
                {
                    HasSomething = false;
                }
            }
            else
            {
                HitInfo2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, CullingMask);

                if (HitInfo2D.collider != null)
                {
                    if (ShowDebug)
                    {
                        Debug.DrawLine(transform.position, HitInfo.point, Color.red, 1);
                    }

                    HasSomething = true;

                    if (On2DHit != null)
                    {
                        On2DHit.Invoke(HitInfo2D);
                    }
                }
                else
                {
                    HasSomething = false;
                }
            }
        }
    }
}