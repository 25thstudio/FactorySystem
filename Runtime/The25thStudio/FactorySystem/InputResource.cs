using System;
using UnityEngine;

namespace The25thStudio.FactorySystem
{
    [Serializable]
    public class InputResource
    {
        [SerializeField]
        private FactoryResource resource;

        [SerializeField]
        private int quantity;


        public bool HasQuantity()
        {
            return resource.HasQuantity(quantity);
        }

        public bool Use()
        {
            return resource.Use(quantity);
        }
    }
}