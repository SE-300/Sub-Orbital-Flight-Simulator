using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

    public class Keyboard : MonoBehaviour {
        private int RCSRemaining = 500; //Just a placeholder for Sprint 2. Will update as implemented
        [SerializeField]
        private int throttleIncrement = 1;
        private int maxThrottle = 100;
        [SerializeField]
        private int throttle;
        [SerializeField]
        private float roll;
        [SerializeField]
        private float pitch;
        [SerializeField]
        private float yaw;
        [SerializeField]
        private float IAS;
        [SerializeField]
        private bool valRCS = false;
        [SerializeField]
        private float impulseRCS = 1179.561615048318f; //Need to verify, current value is in newton meters
        [SerializeField]
        private float Responsiveness = 100000f;
        private float MaximumThrottle = 5255000f; // According to European Space Agency: https://www.esa.int/Science_Exploration/Human_and_Robotic_Exploration/Space_Shuttle/Shuttle_technical_facts
        private Rigidbody rb;
        private GeneralCalculations gc;
        [SerializeField]
        private TextMeshProUGUI PausedText;

        public bool PausedBoolean = false;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>(); // Get the rigidbody so we don't need to manually assign it.
            gc = GetComponent<GeneralCalculations>(); // Get the GeneralCalculations script so we don't need to manually assign it.
        }

        // Update is called once per frame
        void Update()
        {
            gc.setFuel(gc.getFuel() - throttle/ maxThrottle * Time.deltaTime * .1f); // Subtract fuel based on throttle
            HandleInputs();


             if (Input.GetKeyDown(KeyCode.P)) // Pause the game
            {
                if (PausedBoolean == false)
                {
                    Time.timeScale = 0;
                    PausedBoolean = true;
                    PausedText.text = "PAUSED";
                }
                else
                {
                    Time.timeScale = 1;
                    PausedBoolean = false;
                    PausedText.text = " ";

                }
            }

        }   

        // FixedUpdate is called once per physics update; run physics here
        void FixedUpdate() {
            IAS = gc.getIAS();

            applyRoll();
            applyPitch();
            applyYaw();
            applyThrottle();
        }

        private void applyRoll() {
            rb.AddRelativeTorque(Vector3.forward * roll * Mathf.Clamp((IAS-25)/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
        }

        private void applyPitch() {
            rb.AddRelativeTorque(Vector3.right * -pitch * Mathf.Clamp((IAS-25)/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
        }

        private void applyYaw() {
            rb.AddRelativeTorque(Vector3.up * yaw * Mathf.Clamp((IAS-25)/100, 0, 1) * (gc.getDensity()/1.225f) * Responsiveness);
        }

        private void applyThrottle() {
            rb.AddRelativeForce(Vector3.forward * (((float) throttle) / ((float) maxThrottle)) * MaximumThrottle);
        }

        private void HandleInputs() {
            roll = Input.GetAxis("Roll");
            pitch = Input.GetAxis("Pitch");
            yaw = Input.GetAxis("Yaw");

            if(Input.GetKeyDown(KeyCode.T))
            {
                gc.toggleFlapPos();
            }

            if(valRCS != true) //Need to double check movement
            {
                if (Input.GetKey(KeyCode.LeftShift)) throttle += throttleIncrement;
                else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
            } else {
                //Need to populate with movement
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    //Translate X positive
                    rb.AddRelativeForce(Vector3.forward * impulseRCS);
                    gc.updateRCSFuel();
                } 
                if(Input.GetKey(KeyCode.LeftControl))
                {
                    //Translate X negative
                    rb.AddRelativeForce(Vector3.back * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.R))
                {
                    //Translate Y positive
                    rb.AddRelativeForce(Vector3.up* impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.F))
                {
                    //Translate Y negative
                    rb.AddRelativeForce(Vector3.down * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.X))
                {
                    //Translate Z positive
                    rb.AddRelativeForce(Vector3.right * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.Z)) //Need to confirm
                {
                    //Translate Z negative
                    rb.AddRelativeForce(Vector3.left * impulseRCS);
                    gc.updateRCSFuel();
                }


                if(Input.GetKey(KeyCode.S))
                {
                    //Rotate X positive
                    rb.AddRelativeTorque(Vector3.right * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.W))
                {
                    //Rotate X negative
                    rb.AddRelativeTorque(Vector3.left * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.E))
                {
                    //Rotate Y positive
                    rb.AddRelativeTorque(Vector3.up * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.Q))
                {
                    //Rotate Y negative
                    rb.AddRelativeTorque(Vector3.down * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.A)) //Need to check for inversion
                {
                    //Rotate Z positive
                    rb.AddRelativeTorque(Vector3.forward * impulseRCS);
                    gc.updateRCSFuel();
                }
                if(Input.GetKey(KeyCode.D))
                {
                    //Rotate Z negative
                    rb.AddRelativeTorque(Vector3.back * impulseRCS);
                    gc.updateRCSFuel();
                }

            }
            throttle = Math.Clamp(throttle, 0, maxThrottle);

            if(Input.GetKeyDown(KeyCode.CapsLock)) //Flip flop for now, original hypothesis was incorrect
            {
                if(valRCS == true)
                {
                    valRCS = false;
                } else {
                    valRCS = true;
                }
            }
        }

        public int getThrottle()
        {
            return throttle;
        }

        public bool getRCS()
        {
            return valRCS;
        }


    }
