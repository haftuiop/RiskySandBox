using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;

public partial class PrototypingAssets : MonoBehaviour
{
    [SerializeField] bool debugging;

	[SerializeField] ObservableBool PRIVATE_paused;

	[SerializeField] ObservableBool PRIVATE_run_server_code;
	[SerializeField] ObservableBool PRIVATE_run_client_code;
	
	[SerializeField] ObservableFloat PRIVATE_mouse_sensitivity_horizontal;
	[SerializeField] ObservableFloat PRIVATE_mouse_sensitivity_vertical;

	[SerializeField] ObservableBool PRIVATE_is_dedicated_server;


	[SerializeField] int main_menu_scene_ID = -1;

	[SerializeField] int PRIVATE_tick_number;



	private void Awake()
	{
		if (instance != null && instance != this)
		{
			GlobalFunctions.print("PrototypingAssest_GlobalVariables.Awake instance != null... calling Destroy(this)", gameObject);
			Destroy(this);
			return;
		}
		instance = this;

#if UNITY_SERVER
		PRIVATE_is_dedicated_server.value = true;
#else
		PRIVATE_is_dedicated_server.value = false;
#endif
	}



    private void Start()
    {
		if (main_menu_scene_ID == -1)
			return;

		UnityEngine.SceneManagement.SceneManager.LoadScene(main_menu_scene_ID);

	}



    private void Update()
    {
		invokeEvent_Onframe(Time.deltaTime);
    }




}
