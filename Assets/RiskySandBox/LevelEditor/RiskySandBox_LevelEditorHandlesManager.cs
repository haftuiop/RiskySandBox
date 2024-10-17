using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditorHandlesManager : MonoBehaviour
{
    [SerializeField] bool debugging;

    public static RiskySandBox_LevelEditorHandlesManager instance;
    public List<GameObject> hovering_handles = new List<GameObject>();

    public IEnumerable<Vector3> handle_positions { get { return RiskySandBox_LevelEditorHandlesManager.instance.handles.Select(x => x.transform.position); } }

    /// <summary>
    /// invoked whenever a handle is created (or destroyed) or the position changed...
    /// </summary>
    public static event Action OnUpdateHandles;
   

    public ObservableBool enable_MeshRenderer { get { return this.PRIVATE_enable_MeshRenderer; } }
    [SerializeField] ObservableBool PRIVATE_enable_MeshRenderer;

    [SerializeField] MeshFilter my_MeshFilter;


    [SerializeField] GameObject selected_handle;


    public ObservableFloat handle_width { get { return instance.PRIVATE_handle_width; } }
    [SerializeField] ObservableFloat PRIVATE_handle_width;


    [SerializeField] List<LineRenderer> my_LineRenderers = new List<LineRenderer>();

    public ObservableList<GameObject> handles = new ObservableList<GameObject>();


    public GameObject handle_prefab { get { return this.PRIVATE_handle_prefab; } }
    [SerializeField] GameObject PRIVATE_handle_prefab;



    private void Awake()
    {
        instance = this;

        RiskySandBox_LevelEditorHandlesManager.OnUpdateHandles += EventReceiver_OnUpdateHandles;

        RiskySandBox_LevelEditor.Ondisable += EventReceiver_OndisableLevelEditor;

    }

    private void OnDestroy()
    {
        RiskySandBox_LevelEditorHandlesManager.OnUpdateHandles -= EventReceiver_OnUpdateHandles;

        RiskySandBox_LevelEditor.Ondisable -= EventReceiver_OndisableLevelEditor;

    }



    void EventReceiver_OndisableLevelEditor()
    {
        this.destroyAllHandles();
    }

    private void Start()
    {
        redraw();
    }


    private void Update()
    {
        if(this.selected_handle != null)
        {
            this.selected_handle.transform.position = RiskySandBox_CameraControls.mouse_position;
            if (Input.GetMouseButtonUp(0))
            {
                OnUpdateHandles?.Invoke();
                redraw();
                selected_handle = null;
            }
        }

        else
        {

            if (Input.GetMouseButtonDown(0) && hovering_handles.Count > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //create a new handle...
                    //select the new handle...
                    this.selected_handle = this.createHandleAtIndex(hovering_handles[0].transform.position,this.handles.IndexOf(hovering_handles[0]) + 1);

                }
                else
                {
                    this.selected_handle = hovering_handles[0];
                }
            }


        }



        if (Input.GetMouseButtonDown(1))
        {
            if (hovering_handles.Count > 0)
            {
                RiskySandBox_LevelEditorHandlesManager.instance.destroyHandle(hovering_handles[0]);
            }
        }
    }





    void EventReceiver_OnUpdateHandles()
    {
        redraw();
    }


    public void createPermanentLine(float _width,ObservableInt _red, ObservableInt _green, ObservableInt _blue, bool _destroy_handles)
    {
        RiskySandBox_PermanentLine _new_Line = RiskySandBox_Resources.createLine(_width,_red,_green,_blue);

        Vector3[] _points = RiskySandBox_LevelEditorHandlesManager.instance.handle_positions.ToArray();
        _new_Line.line_positions.SET_items(_points);

        if (_destroy_handles)
            this.destroyAllHandles();

    }

    public RiskySandBox_Tile TRY_createTile(int _ID,bool _destroy_handles)
    {
        Vector3[] _points = RiskySandBox_LevelEditorHandlesManager.instance.handle_positions.ToArray();

        if (_points.Count() < 3)
        {
            if (this.debugging)
                GlobalFunctions.print("_points.Count() < 3... retunring null...",this);
            return null;
        }


        RiskySandBox_Tile _created_Tile = RiskySandBox_Resources.createTile(_ID);


        _created_Tile.mesh_points_2D.AddRange(_points);

        if (_destroy_handles)
            this.destroyAllHandles();

        return _created_Tile;
    }


    void redraw()
    {
        if (this.debugging)
            GlobalFunctions.print("redrawing!",this);
        Vector3[] _points = RiskySandBox_LevelEditorHandlesManager.instance.handle_positions.ToArray();

        foreach(LineRenderer _LineRenderer in this.my_LineRenderers)
        {
            if(_LineRenderer == null)
            {
                GlobalFunctions.printWarning("null LineRenderer????",this);
                continue;
            }
            _LineRenderer.positionCount = _points.Count();
            _LineRenderer.SetPositions(_points);
        }

        try
        {
            Mesh _new_Mesh = ShapeCreator.createMesh(_points);
            my_MeshFilter.mesh = _new_Mesh;
        }
        catch
        {

        }
        

        
    }


    public GameObject createHandleAtIndex(Vector3 _point, int _index)
    {
        GameObject _new = UnityEngine.Object.Instantiate(this.handle_prefab);
        _new.transform.position = _point;

        this.handles.Insert(_index, _new);

        OnUpdateHandles?.Invoke();

        return _new;

    }

    public GameObject createHandle(Vector3 _point)
    {
        GameObject _new = UnityEngine.Object.Instantiate(this.handle_prefab);
        _new.transform.position = _point;

        this.handles.Add(_new);

        OnUpdateHandles?.Invoke();

        return _new;
    }

    public void createHandles(IEnumerable<Vector3> _points)
    {
        foreach (Vector3 _v3 in _points)
        {
            GameObject _new = UnityEngine.Object.Instantiate(this.handle_prefab);
            _new.transform.position = _v3;

            this.handles.Add(_new);   
        }

        OnUpdateHandles?.Invoke();
    }


    public void destroyHandle(GameObject _Handle)
    {
        //TODO - make sure it is a handle
        //TODO - make sure it is in the handles list
        //TODO - make sure it isnt null....
        this.handles.Remove(_Handle);

        UnityEngine.Object.Destroy(_Handle.gameObject);

        OnUpdateHandles?.Invoke();
    }

    public void destroyAllHandles()
    {
        foreach (GameObject _Handle in new List<GameObject>(this.handles))
        {
            this.handles.Remove(_Handle);
            UnityEngine.Object.Destroy(_Handle);
        }
        OnUpdateHandles?.Invoke();
    }


}
