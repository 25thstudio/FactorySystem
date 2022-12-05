using UnityEngine;

namespace The25thStudio.FactorySystem
{

    [CreateAssetMenu(fileName = "Factory Resource", menuName = "The 25th Studio/Factory System/Factory Resource", order = 0)]
    public class DefaultFactoryResource : FactoryResource
    {

        [SerializeField] private int quantity;

        public float Quantity => quantity;


        public override void AddQuantity(int value)
        {
            quantity += value;
        }

        public override bool Use(int value)
        {
            if (HasQuantity(value))
            {
                quantity -= value;
                return true;
            }
            return false;
        }

        public override bool HasQuantity(int value)
        {
            return quantity >= value;
        }

    }
}