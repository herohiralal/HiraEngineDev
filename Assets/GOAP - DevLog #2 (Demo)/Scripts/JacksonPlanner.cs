using System;
using UnityEngine;

namespace LGOAPDemo
{
    public class JacksonPlanner : MonoBehaviour
    {
        [SerializeField] private JacksonBlackboard blackboard = default;
        private int _instanceID;

        private void Awake()
        {
            _instanceID = GetInstanceID();
            
        }

        private void OnDestroy()
        {
            _instanceID = int.MinValue;
        }

        public float Health
        {
            get => blackboard.health;
            set
            {
                blackboard.health = value;
                OnBlackboardValueUpdate();
            }
        }

        public float Mana
        {
            get => blackboard.mana;
            set
            {
                blackboard.mana = value;
                OnBlackboardValueUpdate();
            }
        }

        public float Stamina
        {
            get => blackboard.stamina;
            set
            {
                blackboard.stamina = value;
                OnBlackboardValueUpdate();
            }
        }

        public float GetFloatValue(string keyName) =>
            keyName switch
            {
                "Health" => Health,
                "Mana" => Mana,
                "Stamina" => Stamina,
                _ => throw new ArgumentOutOfRangeException($"{keyName} could not be found.")
            };

        public void SetFloatValue(string keyName, float value)
        {
            switch (keyName)
            {
                case "Health":
                    Health = value;
                    break;
                case "Mana":
                    Mana = value;
                    break;
                case "Stamina":
                    Stamina = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{keyName} could not be found.");
            }
        }

        private void OnBlackboardValueUpdate()
        {
        }
    }
}