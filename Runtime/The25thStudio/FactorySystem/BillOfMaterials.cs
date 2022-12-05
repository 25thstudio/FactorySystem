using System.Collections.Generic;
using UnityEngine;

namespace The25thStudio.FactorySystem
{
    [CreateAssetMenu(fileName = "BOM", menuName = "The 25th Studio/Factory System/BOM", order = 0)]
    public class BillOfMaterials : ScriptableObject
    {
        [SerializeField]
        private List<InputResource> input;

        [SerializeField]
        private FactoryResource output;

        [SerializeField, Min(0)]
        private int quantity;

        [SerializeField, Min(0)]
        private float timeInSeconds;
        
        public float TimeInSeconds => timeInSeconds;


        internal bool IsTimeToMake(float time)
        {
            return time > timeInSeconds;
        }
        internal bool CanMake()
        {
            return InputListIsEmpty() || CheckInputQuantity();
        }

        internal void Consume()
        {
            // Use the input resource
            input.ForEach(ir => ir.Use());
        }

        internal void Make(out FactoryResource outputResource, out int outputQuantity)
        {
            
            // Increase the output quantity
            output.AddQuantity(quantity);
            
            // Set the outputvalues
            outputResource = output;
            outputQuantity = quantity;

        }



        private bool InputListIsEmpty()
        {
            return input.Count == 0;
        }

        private bool CheckInputQuantity()
        {
            return input.TrueForAll(r => r.HasQuantity());
        }


    }
}