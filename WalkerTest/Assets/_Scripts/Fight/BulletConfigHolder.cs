using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scene.Fight
{
    public interface IBulletConfigHolder
    {
        IBulletConfig GetConfigById(string id);

	}

    [CreateAssetMenu(
		fileName = "BulletConfigHolder",
		menuName = "ScriptableObjects/BulletConfigHolder",
		order = 1)]
	public class BulletConfigHolder : ScriptableObject, IBulletConfigHolder
	{
        [SerializeField] private BulletConfig[] _configs;
        
        public IBulletConfig GetConfigById(string id)
        {
            return _configs.FirstOrDefault(b => b.Key.Equals(id));
        }
    }
}