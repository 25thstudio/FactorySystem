using UnityEngine;
using UnityEngine.Events;

namespace The25thStudio.FactorySystem
{
    public class Factory : MonoBehaviour
    {
        [SerializeField, Tooltip("The bill of materials to make new resources")]
        private BillOfMaterials bom;

        [Header("Cool Down settings")]
        [SerializeField, Min(0), Tooltip("Time in seconds to wait before checking if the resources are available")]
        private float coolDownInSeconds = 5f;


        [Header("Queue Factory")]
        [SerializeField, Tooltip("If this factory is queue based")]
        private bool queueFactory = false;

        [Header("Events")]
        [SerializeField, Tooltip("Event when an item is released")]
        private UnityEvent<FactoryResource, int> release;

        [Header("Animation")]        
        [SerializeField, Tooltip("The animator controller to update the parameters")]
        private Animator animator;
        [SerializeField, Tooltip("The current 'percentage' value, Int")]
        private string percentageAnimationParameter = "percentage";
        [SerializeField, Tooltip("If the factory is idle, bool")]
        private string idleAnimationParameter = "idle";
        [SerializeField, Tooltip("If the factory is working, bool")]
        private string workingAnimationParameter = "working";
        [SerializeField, Tooltip("If the factory is in cool down mode, bool")]
        private string coolDownAnimationParameter = "coolDown";
        [SerializeField, Tooltip("If the factory is queue based, bool")]
        private string queueFactoryAnimationParameter = "queueFactory";
        [SerializeField, Tooltip("The factory queue size, int")]
        private string queueSizeAnimationParameter = "queueSize";



        private int _queueSize = 0;
        private float _time = 0f;
        private float _percentage = 0f;
        private float _coolDownTime = 0f;


        private FactoryStatus _status = FactoryStatus.IDLE;
        private string _statusMessage = "";


        private void Update()
        {
            UpdateAnimationParameters();
            switch (_status)
            {
                case FactoryStatus.IDLE: Idle(); break;
                case FactoryStatus.WORKING: Working(); break;
                case FactoryStatus.COOL_DOWN: CoolDown(); break;
            }
        }

        private void Idle()
        {
            if (HasConditionToStartWorking())
            {
                var bomCanMake = bom.CanMake();
                _status = bomCanMake ? StartWorking() : StartCoolDown();
            }
            else
            {
                _statusMessage = "Queue factory with empty queue";
            }

        }

        private void Working()
        {
            IncreaseTimer();
            if (bom.IsTimeToMake(_time))
            {
                ResetTimer();
                bom.Make(out var resource, out var quantity);
                InvokeReleaseEvent(resource, quantity);
                _status = FactoryStatus.IDLE;
            }
            else
            {
                _statusMessage = $"Working {_percentage}%";
            }
        }

        private void CoolDown()
        {
            IncreaseCoolDownTimer();
            if (IsTimeToCoolDownOff())
            {
                _status = CheckIfCanDisableCoolDown();
            }
            else
            {
                _statusMessage = $"Cool down mode ON. Time {_coolDownTime}";
            }

        }

        #region Public methods

        public float Percentage => _percentage;
        public int QueueSize => _queueSize;

        public FactoryStatus Status => _status;

        public string StatusMessage => _statusMessage;

        public void Queue()
        {
            _queueSize++;
        }

        #endregion

        #region Animation 
        private void UpdateAnimationParameters()
        {
            if (animator == null) return;

            // Status parameters
            animator.SetBool(idleAnimationParameter, _status == FactoryStatus.IDLE);
            animator.SetBool(workingAnimationParameter, _status == FactoryStatus.WORKING);
            animator.SetBool(coolDownAnimationParameter, _status == FactoryStatus.COOL_DOWN);

            // Working percentage
            animator.SetFloat(percentageAnimationParameter, _percentage);

            // Queue
            animator.SetBool(queueFactoryAnimationParameter, queueFactory);
            animator.SetInteger(queueSizeAnimationParameter, _queueSize);
        }
        #endregion


        private bool HasConditionToStartWorking()
        {
            if (queueFactory)
            {
                return _queueSize > 0;
            }
            return true;
        }

        private FactoryStatus StartCoolDown()
        {
            _statusMessage = "No enough resources. Starting cool down mode";
            return FactoryStatus.COOL_DOWN;
        }
        private FactoryStatus StartWorking()
        {
            if (queueFactory)
            {
                // Consume the queue if there is item to consume
                _queueSize--;
            }
            // Consume the bom resources
            bom.Consume();
            // Change the status to working
            return FactoryStatus.WORKING;
        }

        private void IncreaseTimer()
        {
            _time += Time.deltaTime;
            _percentage = _time / bom.TimeInSeconds * 100;
        }

        private void IncreaseCoolDownTimer()
        {
            _coolDownTime += Time.deltaTime;
        }

        private void InvokeReleaseEvent(FactoryResource resource, int quantity)
        {
            release.Invoke(resource, quantity);
        }

        private void ResetTimer()
        {
            _time = 0f;
            _percentage = 0f;
        }

        private bool IsTimeToCoolDownOff()
        {
            return _coolDownTime > coolDownInSeconds;
        }

        private FactoryStatus CheckIfCanDisableCoolDown()
        {
            _coolDownTime = 0f;
            return bom.CanMake() ? FactoryStatus.IDLE : FactoryStatus.COOL_DOWN;


        }

        public enum FactoryStatus
        {
            // Queue is empty
            IDLE,
            // Working on an item
            WORKING,
            // Not wnough resources
            COOL_DOWN
        }


    }
}