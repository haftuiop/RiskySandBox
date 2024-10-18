using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class ObservableVector3List
{
    public event Action<ObservableVector3List> OnUpdate;
    public event Action<ObservableVector3List> Onsynchronize;


    public void synchronize()
    {
        if (debugging)
            GlobalFunctions.print("called synchronize!", this);

        Onsynchronize?.Invoke(this);
    }


}
