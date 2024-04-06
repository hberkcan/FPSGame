using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Singleton<Factory>
{
    public ProjectileFactory ProjectileFactory { get; private set; } = new ProjectileFactory();
}
