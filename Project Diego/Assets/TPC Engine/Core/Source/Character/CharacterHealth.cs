/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using System.Collections;
using TPCEngine.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace TPCEngine
{
    /// <summary>
    /// Fal Damage struct
    /// </summary>
    [System.Serializable]
    public struct FallDamageParam
    {
        public float minHeight;
        public float maxHeight;
        public int damage;
    }

    [System.Serializable]
    public struct RegenirationParam
    {
        public float interval;
        public int value;
        public float time;
    }

    /// <summary>
    /// Base Player Health class
    /// </summary>
    public class CharacterHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;
        [SerializeField] private int startHealth;
        [SerializeField] private FallDamageParam[] fallDamageParams;
        [SerializeField] private Image damageImage;
        [SerializeField] private bool useRegeniration;
        [SerializeField] private RegenirationParam regenerationParam;

        private Animator animator;
        private TPCamera characterCamera;
        private TPCMotor characterMotor;
        private float lastHeightPosition;
        private bool onceHealth;
        private bool onceRegeneration;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            characterMotor = GetComponent<TPCharacter>().GetCharacteMotor();
            characterCamera = GetComponent<TPCharacter>().GetCamera();
            transform.SetKinematic(true);
            health = startHealth;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            HealthHandler();
            DamageScreenHandler();

            if (IsAlive)
            {
                HealthRegenerationHandler();
                FallDamage(characterMotor.IsGrounded);
            }
        }

        /// <summary>
        /// Character Health handler
        /// </summary>
        public virtual void HealthHandler()
        {
            if (!IsAlive)
            {
                characterMotor.LockMovement = true;
                characterCamera.SetTarget(animator.GetBoneTransform(HumanBodyBones.Spine));
                Ragdoll(true);
                onceHealth = true;
            }
            else if (onceHealth)
            {
                characterMotor.LockMovement = false;
                characterCamera.SetTarget(transform);
                Ragdoll(false);
                onceHealth = false;
            }
        }

        /// <summary>
        /// Health Regenegation System
        /// </summary>
        public virtual void HealthRegenerationHandler()
        {
            if (IsAlive && useRegeniration && health != maxHealth && !onceRegeneration)
            {
                StartCoroutine(RegenerateProcess(regenerationParam));
                onceRegeneration = true;
            }

        }

        public IEnumerator RegenerateProcess(RegenirationParam regenerationParam)
        {
            bool waitBeforeStart = true;
            bool playRegenerate = false;

            while (true)
            {
                //First thread
                while (waitBeforeStart)
                {
                    float lastHealth = health;
                    yield return new WaitForSeconds(regenerationParam.time);
                    if (lastHealth == health)
                    {
                        waitBeforeStart = false;
                        playRegenerate = true;
                        break;
                    }
                }

                //Second thread
                while (playRegenerate)
                {
                    health += regenerationParam.value;
                    float lastHealth = health;
                    yield return new WaitForSeconds(regenerationParam.interval);
                    if (health == maxHealth)
                    {
                        onceRegeneration = false;
                        yield break;
                    }
                    else if (lastHealth != health)
                    {
                        waitBeforeStart = true;
                        playRegenerate = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Damage Screen System Handler
        /// </summary>
        public void DamageScreenHandler()
        {
            if (damageImage == null)
                return;

            if (damageImage.color.a != (float) (100 - health) / 100)
            {
                damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, Mathf.MoveTowards(damageImage.color.a, (float) (100 - health) / 100, 10 * Time.deltaTime));
            }
        }

        /// <summary>
        /// Take damage
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            health -= amount;
        }

        public virtual void FallDamage(bool isGrounded)
        {
            if (!isGrounded)
            {
                if (lastHeightPosition < transform.position.y)
                {
                    lastHeightPosition = transform.position.y;
                }
            }
            else if (lastHeightPosition > transform.position.y)
            {
                float distance = lastHeightPosition - transform.position.y;
                for (int i = 0; i < fallDamageParams.Length; i++)
                {
                    if (distance > fallDamageParams[i].minHeight && distance < fallDamageParams[i].maxHeight)
                    {
                        TakeDamage(fallDamageParams[i].damage);
                        lastHeightPosition = transform.position.y;
                    }
                }
            }
        }

        public virtual void Ragdoll(bool isStart)
        {
            transform.SetKinematic(!isStart);
            animator.enabled = !isStart;
        }

        /// <summary>
        /// Player health
        /// </summary>
        public int Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
            }
        }

        /// <summary>
        /// Player is alive
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return (health > 0) ? true : false;
            }
        }

        public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

        public float HealthPercent { get { return ((float) health / maxHealth) * 100; } }

    }
}