using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Fight
{
    public interface IShootable
    {
        Health Health { get; }
        Collider2D Collider { get; }
		string Hash { get; }
        TargetType TargetType { get; }
	}
}