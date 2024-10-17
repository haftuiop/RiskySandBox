using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public static event Action<Missile> OnComplete_STATIC;
    public static ObservableList<Missile> all_instances = new ObservableList<Missile>();



    [SerializeField] Vector3 targetPosition;  // The target position the object will move towards
    public float height { get { return Vector3.Distance(startPosition, targetPosition) * 0.5f; } }     // The peak height of the parabola
    public float speed = 1.0f;      // The speed of movement

    [SerializeField] Vector3 startPosition;  // The start position of the object
    private float time = 0.0f;      // Time tracker for the movement

    [SerializeField] GameObject end_of_motion_GameObject;

    Vector3 previousPosition = new Vector3(0, 0, 0);

    [SerializeField] bool destroy_gameObject_at_Oncomplete;

    public AudioClip on_complete_AudioClip;

    // Define the OnComplete event
    public event Action OnComplete;


    private void Awake()
    {
        Missile.all_instances.Add(this);

        this.OnComplete += EventReceiver_Oncomplete;
    }

    private void OnDestroy()
    {
        if(Missile.all_instances.Contains(this))
            Missile.all_instances.Remove(this);
    }

    void Start()
    {
        transform.position = startPosition;
    }

    void Update()
    {
        // Increment time based on speed
        time += Time.deltaTime * speed;

        // Calculate the linear interpolation between start and target positions
        Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, time);

        // Calculate the height of the parabola at this point
        float parabolaHeight = Mathf.Sin(Mathf.Clamp01(time) * Mathf.PI) * height;

        // Adjust the current position with the parabolic height
        currentPosition.y += parabolaHeight;

        // Make the object look in the direction of travel
        Vector3 directionOfTravel = currentPosition - previousPosition;
        if (directionOfTravel != Vector3.zero) // Ensure direction is valid
        {
            transform.rotation = Quaternion.LookRotation(directionOfTravel);
        }

        // Update the object's position
        transform.position = currentPosition;

        // Update the previous position to the current position
        previousPosition = currentPosition;

        // Stop moving when the object reaches the target position
        if (time >= 1.0f)
        {
            // Disable the script
            enabled = false;

            // Invoke the OnComplete event if there are subscribers
            Missile.OnComplete_STATIC?.Invoke(this);
            this.OnComplete?.Invoke();
        }
    }

    void EventReceiver_Oncomplete()
    {
        if (this.end_of_motion_GameObject != null)
        {
            GameObject _new = UnityEngine.Object.Instantiate(this.end_of_motion_GameObject);
            _new.gameObject.SetActive(true);
            _new.transform.position = targetPosition;
        }

        if (this.destroy_gameObject_at_Oncomplete)
            UnityEngine.Object.Destroy(this.gameObject);

        //this.end_of_motion_GameObject.SetActive(true);
        //destroy after a certain time???
    }


    public static Missile createNew(GameObject _prefab,Vector3 _start,Vector3 _end,float _speed)
    {
        GameObject _new = UnityEngine.Object.Instantiate(_prefab);

        Missile _Missile_script = _new.GetComponent<Missile>();
        if (_Missile_script == null)
            _Missile_script = _prefab.AddComponent<Missile>();

        _Missile_script.startPosition = _start;
        _Missile_script.targetPosition = _end;
        _Missile_script.speed = _speed;

        return _Missile_script;
    }

    public static Missile createNew_duration(GameObject _prefab,Vector3 _start,Vector3 _end,float _duration)
    {
        GameObject _new = UnityEngine.Object.Instantiate(_prefab);

        Missile _Missile_script = _new.GetComponent<Missile>();
        if (_Missile_script == null)
            _Missile_script = _prefab.AddComponent<Missile>();

        _Missile_script.startPosition = _start;
        _Missile_script.targetPosition = _end;
        _Missile_script.speed = Vector3.Distance(_start, _end) / _duration;

        return _Missile_script;
    }
}

