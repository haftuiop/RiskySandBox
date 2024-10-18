using System;

public partial class ObservableVector3
{
	public static implicit operator string(ObservableVector3 _ObservableVector3) { return "("+ _ObservableVector3.x + ", "+ _ObservableVector3.y+", "+ _ObservableVector3.z+")"; }//converts the _ObservableVector3 into a string

	public event Action<ObservableVector3> OnUpdate;
	public event Action<ObservableVector3> Onsynchronize;//the code that needs to run in order to sync this value to the clients (or server)

	public float x
	{
		get{ return this.value[0]; }
		set{this.SET_x(value);}
	}
	public float X
	{
		get { return this.value[0]; }
		set { this.SET_x(value); }
	}

	public float y
	{
		get { return this.value[1]; }
		set { this.SET_y(value); }
	}
	public float Y
	{
		get { return this.value[1]; }
		set { this.SET_y(value); }
	}

	public float z
	{
		get { return this.value[2]; }
		set { this.SET_z(value); }
	}
	public float Z
	{
		get { return this.value[2]; }
		set { this.SET_z(value); }
	}


	public void synchronize()
	{
		if (debugging)
			GlobalFunctions.print("called synchronize!", this);
		Onsynchronize?.Invoke(this);
	}


}

