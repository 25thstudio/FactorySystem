using UnityEngine;


namespace The25thStudio.FactorySystem
{
    
    public abstract class FactoryResource: ScriptableObject
    {
        [SerializeField] private string resourceName;


        public string ResourceName => resourceName;


        public abstract void AddQuantity(int value);

        public abstract bool Use(int value);

        public abstract bool HasQuantity(int value);

    }

}
