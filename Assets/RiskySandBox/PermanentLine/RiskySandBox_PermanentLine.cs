using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_PermanentLine : MonoBehaviour
{
    //TODO add in a setting for what type of line e.g. dotted, solid etc...
    public static ObservableList<RiskySandBox_PermanentLine> all_instances = new ObservableList<RiskySandBox_PermanentLine>();

    [SerializeField] bool debugging;

    [SerializeField] LineRenderer my_LineRenderer;


    public ObservableFloat line_width { get { return this.PRIVATE_line_width; } }
    [SerializeField] ObservableFloat PRIVATE_line_width;

    public ObservableVector3List line_positions { get { return this.PRIVATE_line_positions; } }
    [SerializeField] ObservableVector3List PRIVATE_line_positions;


    public ObservableInt my_Color_r { get { return this.PRIVATE_my_Color_r; } }
    [SerializeField] ObservableInt PRIVATE_my_Color_r;

    public ObservableInt my_Color_g { get { return this.PRIVATE_my_Color_g; } }
    [SerializeField] ObservableInt PRIVATE_my_Color_g;

    public ObservableInt my_Color_b { get { return this.PRIVATE_my_Color_b; } }
    [SerializeField] ObservableInt PRIVATE_my_Color_b;


    private void Awake()
    {
        RiskySandBox_PermanentLine.all_instances.Add(this);

        my_LineRenderer.material = new Material(Shader.Find("Standard"));
        RiskySandBox_Map.OnsaveMap += EventReceiver_OnsaveMap;
        RiskySandBox_Map.OnclearMap += EventReceiver_OnclearMap;

        this.line_positions.OnUpdate += delegate { updateVisuals(); };
        this.my_Color_r.OnUpdate += delegate { updateVisuals(); };
        this.my_Color_g.OnUpdate += delegate { updateVisuals(); };
        this.my_Color_b.OnUpdate += delegate { updateVisuals(); };
        this.line_width.OnUpdate += delegate { updateVisuals(); };

    }
    private void OnDestroy()
    {
        RiskySandBox_PermanentLine.all_instances.Remove(this);

        RiskySandBox_Map.OnsaveMap -= EventReceiver_OnsaveMap;
        RiskySandBox_Map.OnclearMap -= EventReceiver_OnclearMap;

    }

    private void Start()
    {
        updateVisuals();
    }

    void updateVisuals()
    {
        this.my_LineRenderer.startWidth = this.line_width;
        this.my_LineRenderer.endWidth = this.line_width;
        my_LineRenderer.material.color = new Color(my_Color_r / 255f, my_Color_g / 255f, my_Color_b / 255f);
        this.my_LineRenderer.positionCount = this.line_positions.Count();
        this.my_LineRenderer.SetPositions(this.line_positions.ToArray());

    }



    void EventReceiver_OnclearMap()
    {
        UnityEngine.Object.Destroy(this.gameObject);
    }

    void EventReceiver_OnsaveMap(string _directory)
    {
        //ok lets save ourselves into the directory...
        int _ID = all_instances.IndexOf(this);
        string _file = System.IO.Path.Combine(_directory, string.Format("Line_{0}.txt", _ID));

        System.IO.StreamWriter _writer = new System.IO.StreamWriter(_file);

        _writer.WriteLine(string.Format("my_Color:{0},{1},{2}", this.my_Color_r.value, this.my_Color_g.value, this.my_Color_b.value));
        _writer.WriteLine(string.Format("line_width:{0}", this.line_width.value));
        _writer.WriteLine(string.Format("verts:{0}", this.line_positions.exportToFloatString()));

        _writer.Close();

    }



    public static RiskySandBox_PermanentLine loadLine(string[] _lines)
    {
        //ok get the line width...
        //get the points]
        //get the color
        Dictionary<string, string> _data = new Dictionary<string, string>();

        float _width = 1f;

        int _r = 0;
        int _g = 0;
        int _b = 0;

        
        foreach (string _line in _lines)
        {
            _data.Add(_line.Split(':')[0], _line.Split(':')[1]);
        }

        if(_data.ContainsKey("my_Color") == true)
        {
            int[] _color_values = _data["my_Color"].Split(',').Select(x => int.Parse(x)).ToArray();
            _r = _color_values[0];
            _g = _color_values[1];
            _b = _color_values[2];
        }

        if(_data.ContainsKey("line_width") == true)
        {
            _width = float.Parse(_data["line_width"]);
        }

        RiskySandBox_PermanentLine _new_Line = RiskySandBox_Resources.createLine(_width, _r, _g, _b);

        if (_data.ContainsKey("verts") == true)
        {
            _new_Line.line_positions.importFromFloatString(_data["verts"]);
        }

        return _new_Line;
    }
}
