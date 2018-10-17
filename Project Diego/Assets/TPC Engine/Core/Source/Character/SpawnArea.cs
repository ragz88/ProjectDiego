/* ================================================================
   ---------------------------------------------------
   Project   :    TPC Engine
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Favilevich
   ---------------------------------------------------
   Copyright © Tamerlan Favilevich 2017 - 2018 All rights reserved.
   ================================================================ */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TPCEngine.Utility;

namespace TPCEngine
{
    /// <summary>
    /// Spawn area shape
    /// </summary>
    public enum SpawnShape { Rectangle, Circle }

    /// <summary>
    /// Character spawn manager
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SpawnArea : MonoBehaviour
    {
        [SerializeField] private SpawnShape shape;
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 rotation;
        [SerializeField] private float radius = 1;
        [SerializeField] private float lenght = 1;
        [SerializeField] private float weight = 1;
        [SerializeField] private float time;
        [SerializeField] private bool autoSpawn;
        [SerializeField] private KeyCode spawnKey;
        [SerializeField] private AudioClip sound;

        [SerializeField] private Transform panel;
        [SerializeField] private Text message;

        private IHealth health;
        private AudioSource audioSource;
        private float displayTime;
        private bool isCalled;
        private bool doSpawn;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            health = player.GetComponent<IHealth>();
            audioSource = GetComponent<AudioSource>();
            displayTime = time;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (health != null && !health.IsAlive)
            {
                if (!isCalled)
                {
                    Spawn(time);
                    isCalled = true;
                }
                displayTime -= Time.deltaTime;
                if (autoSpawn)
                {
                    message.text = "Respawn after: " + ((displayTime >= 0) ? displayTime : 0).ToString("#");
                }
                else
                {
                    if (displayTime >= 0)
                    {
                        message.text = "Respawn after: " + ((displayTime >= 0) ? displayTime : 0).ToString("#");
                    }
                    else
                    {
                        message.text = "Respawn after: Press the " + "\"" + spawnKey.ToString() + "\"" + " Key";
                        if (Input.GetKeyDown(spawnKey))
                        {
                            panel.gameObject.SetActive(false);
                        }
                    }
                }
                if (!panel.gameObject.activeSelf)
                {
                    panel.gameObject.SetActive(true);
                }
            }
            else if (panel.gameObject.activeSelf)
            {
                panel.gameObject.SetActive(false);
                displayTime = time;
                isCalled = false;
            }
        }

        /// <summary>
        /// Spawn Player
        /// </summary>
        public virtual void Spawn(float time)
        {
            StartCoroutine(SpawnHandler(time));
        }

        /// <summary>
        /// Instantiate a player after a specified time
        /// </summary>
        /// <param name="time"></param>
        /// <returns>IEnumerator</returns>
        public virtual IEnumerator SpawnHandler(float time)
        {
            yield return new WaitForSeconds(time);
            while (true)
            {
                if (autoSpawn || (!autoSpawn && Input.GetKeyDown(spawnKey)))
                    doSpawn = true;
                if (doSpawn)
                {
                    health.Health = health.MaxHealth;
                    Vector3 randomPoint = transform.position;
                    switch (shape)
                    {
                        case SpawnShape.Rectangle:
                            randomPoint = randomPoint.RandomPositionInRectangle(lenght, weight);
                            break;
                        case SpawnShape.Circle:
                            randomPoint = randomPoint.RandomPositionInCircle(radius);
                            break;
                    }
                    player.SetPositionAndRotation(randomPoint, Quaternion.Euler(rotation));
                    audioSource.PlayOneShot(sound);
                    doSpawn = false;
                    yield break;
                }
                yield return null;
            }

        }

        /// <summary>
        /// Player rotation
        /// </summary>
        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation = value;
            }
        }

        /// <summary>
        /// Spawn zone radius
        /// </summary>
        public float Radius
        {
            get
            {
                return radius;
            }

            set
            {
                radius = value;
            }
        }

        /// <summary>
        /// Time before spawn
        /// </summary>
        public float SpawnTime
        {
            get
            {
                return time;
            }

            set
            {
                time = value;
            }
        }

        /// <summary>
        /// Auto spawn
        /// </summary>
        public bool AutoSpawn
        {
            get
            {
                return autoSpawn;
            }

            set
            {
                autoSpawn = value;
            }
        }

        /// <summary>
        /// Spawn Key
        /// </summary>
        public KeyCode SpawnKey
        {
            get
            {
                return spawnKey;
            }

            set
            {
                spawnKey = value;
            }
        }

        /// <summary>
        /// Spawn sound
        /// </summary>
        public AudioClip Sound
        {
            get
            {
                return sound;
            }

            set
            {
                sound = value;
            }
        }

        /// <summary>
        /// Spawn message
        /// </summary>
        public Text Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        /// <summary>
        /// Player Health
        /// </summary>
        public IHealth Health
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
        /// Audio Source
        /// </summary>
        public AudioSource AudioSource
        {
            get
            {
                return audioSource;
            }

            set
            {
                audioSource = value;
            }
        }

        public SpawnShape Shape { get { return shape; } set { shape = value; } }

        public float Lenght { get { return lenght; } set { lenght = value; } }

        public float Weight { get { return weight; } set { weight = value; } }
    }
}