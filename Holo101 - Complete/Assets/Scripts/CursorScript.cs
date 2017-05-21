using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class CursorScript : MonoBehaviour
{
    public Material ActiveCursorMaterial;
    public Material ActiveCursorMaterialGravityOff;
    public Material InactiveCursorMaterial;
    public GameObject TheCursor;

    private Material _activeCursorMaterial;
    private GameObject _focusedObject;
    private GestureRecognizer _gestureRecognizer;
    private RaycastHit _hitInfo;
    private bool _isGravitySwitchOn;

    public static CursorScript Instance
    {
        get;
        private set;
    }

    public bool IsGravitySwitchOn
    {
        get
        {
            return _isGravitySwitchOn;
        }
        set
        {
            _isGravitySwitchOn = value;
            _activeCursorMaterial = _isGravitySwitchOn 
                ? ActiveCursorMaterial : ActiveCursorMaterialGravityOff;
        }
    }

    public CursorScript()
    {
        Instance = this;
    }

    private void GestureRecognizerOnTappedEvent(InteractionSourceKind source, int tapcount, Ray headray)
    {
        if (_focusedObject == null)
        {
            return;
        }

        var rigidBody = _focusedObject.GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            if (rigidBody.useGravity
                || !IsGravitySwitchOn)
            {
                rigidBody.AddForce(-_hitInfo.normal * 100f);
            }
            else
            {
                rigidBody.useGravity = true;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.TappedEvent += GestureRecognizerOnTappedEvent;
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _gestureRecognizer.StartCapturingGestures();

        IsGravitySwitchOn = true;
        _activeCursorMaterial = ActiveCursorMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (TheCursor == null)
        {
            return;
        }

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        var cursorRenderer = TheCursor.GetComponent<Renderer>();

        if (Physics.Raycast(headPosition, gazeDirection, out _hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            // But not the spatial mapping!
            var obj = _hitInfo.collider.gameObject;

            if (!obj.name.StartsWith("spatial")) // Fugly...
            {
                _focusedObject = obj;
                cursorRenderer.material = _activeCursorMaterial;
            }
            else
            {
                _focusedObject = null;
                cursorRenderer.material = InactiveCursorMaterial;
            }

            TheCursor.transform.position = _hitInfo.point;
            TheCursor.transform.forward = _hitInfo.normal;
        }
    }
}