using System.Collections.Generic;
using UnityEngine;

public class HitstopOnCollide : ActionOnCollide {
    
    protected override void DoAction() {
        Hitstop.Instance.DoHitstop();
    }
}
