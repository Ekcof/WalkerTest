using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scene.Fight
{
    public interface IGunner
    {
		Collider2D Collider  { get; }
		IGun Gun { get; }
	}
}