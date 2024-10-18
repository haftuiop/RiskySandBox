using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class PrototypingAssets
{
	static PrototypingAssets instance;


	/// <summary>
	/// some code should only be run on the server... e.g. damaging objects
	/// </summary>
	public static ObservableBool run_server_code { get { return instance.PRIVATE_run_server_code; } }

	/// <summary>
	/// some code doesnt need to run unless we are the 'clients' e.g. damage vfx or ui updates...
	/// </summary>
	public static ObservableBool run_client_code { get { return instance.PRIVATE_run_client_code; } }


	/// <summary>
	/// is the game currently paused...
	/// </summary>
	public static ObservableBool paused { get { return instance.PRIVATE_paused; } }

	public static ObservableFloat mouse_sensitivity_horizontal { get { return instance.PRIVATE_mouse_sensitivity_horizontal; } }
	public static ObservableFloat mouse_sensitivity_vertical { get { return instance.PRIVATE_mouse_sensitivity_vertical; } }

	/// <summary>
    /// is the build of this game a dedicated server
    /// </summary>
	public static ObservableBool is_dedicated_server { get { return instance.PRIVATE_is_dedicated_server; } }




	//events...

	/// <summary>
    /// invoked just before Onframe
    /// </summary>
	public static event Action<EventInfo_Onframe> Onframe_pre;

	/// <summary>
    /// should be invoked every single frame
    /// </summary>
	public static event Action<EventInfo_Onframe> Onframe;

	/// <summary>
    /// invoked just after Onframe
    /// </summary>
	public static event Action<EventInfo_Onframe> Onframe_post;


	/// <summary>
    /// invoked just before Ontick
    /// </summary>
	public static event Action<EventInfo_Ontick> Ontick_pre;

	/// <summary>
    /// should be invoked every time a game event (independent of frame rate)
    /// </summary>
	public static event Action<EventInfo_Ontick> Ontick;

	/// <summary>
    /// invoked just after Ontick
    /// </summary>
	public static event Action<EventInfo_Ontick> Ontick_post;

	public static int tick_number { get { return instance.PRIVATE_tick_number; } }


	public void tick()
    {
		GlobalFunctions.print("called tick", this);

		EventInfo_Ontick _EventInfo = new EventInfo_Ontick(tick_number);

		Ontick_pre?.Invoke(_EventInfo);
		Ontick?.Invoke(_EventInfo);
		Ontick_post?.Invoke(_EventInfo);

		this.PRIVATE_tick_number += 1;
		//TODO - this maybe needs to be a long... not an just to be sure it is fine
    }

	public struct EventInfo_Ontick
	{
		public EventInfo_Ontick(int _tick_number)
		{
			this.tick_number = _tick_number;
		}
		public readonly int tick_number;
	}


	void invokeEvent_Onframe(float _delta_time)
	{
		if(this.debugging)
			GlobalFunctions.print("invoking the frame event", this);

		EventInfo_Onframe _EventInfo = new EventInfo_Onframe(_delta_time, this.PRIVATE_paused.value);

		Onframe_pre?.Invoke(_EventInfo);
		Onframe?.Invoke(_EventInfo);
		Onframe_post?.Invoke(_EventInfo);
	}




	public struct EventInfo_Onframe
	{
		public EventInfo_Onframe(float _delta_time,bool _paused)
		{
			this.delta_time = _delta_time;
			this.paused = _paused;
			this.deltaTime = _delta_time;
		}
		public readonly float delta_time;
		public readonly bool paused;
		/// <summary>
		/// exactly the same as delta_time just with a different name (redundant)
		/// </summary>
		public readonly float deltaTime;
	}



}

