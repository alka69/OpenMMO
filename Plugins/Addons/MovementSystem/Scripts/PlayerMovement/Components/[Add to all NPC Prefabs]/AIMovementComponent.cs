// by Fhiz, DX4D
using UnityEngine;
using Mirror;

namespace OpenMMO
{

    /// <summary>
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    [System.Serializable]
    public partial class AIMovementComponent : EntityMovementComponent
    {
        [Header("Movement Profile")]
        [SerializeField] MovementProfile input;

        private void OnValidate()
        {
            if (!input) input = Resources.Load<AIMovementProfile>("Player/Movement/_default/DefaultMovementProfile-AI"); //LOAD MOVEMENT PROFILE
        }
        //- - - - - - - -
        //MOVEMENT STATES
        //- - - - - - - -
        // MovementStates represent the various movement related states of a character.
        // Note that certain states like Stealth can be active at the same time as other states like Running.

        //IDLE?
        /// <summary>Checks if the character is standing still</summary>
        public bool Idle { get { return !Moving; } }
        //MOVING?
        /// <summary>Checks if the character is moving</summary>
        public bool Moving { get { return agent.velocity != Vector3.zero; } }
        //WALKING?
        /// <summary>Checks if the character is walking</summary>
        public bool Walking { get { return Moving && !input.running; } }
        //RUNNING?
        /// <summary>Checks if the character is running</summary>
        public bool Running { get { return Moving && input.running; } }
        //SNEAKING?
        /// <summary>Checks if the character is moving stealthily</summary>
        public bool Sneaking { get { return input.sneaking; } }


        /// <summary>
    	/// Start called client and server-side to initialize properties.
    	/// </summary>
        protected override void Start()
        {
            agent.updateRotation = false;
            base.Start();
        }

        /// <summary>
    	/// OnStartLocalPlayer. Not used yet.
    	/// </summary>
        public override void OnStartLocalPlayer()
        {
        }

        /// <summary>
    	/// OnDestroy client and side-server. Not used yet.
    	/// </summary>
        void OnDestroy()
        {
        }

        /// <summary>
    	/// Server-side throttled update.
    	/// </summary>
        [Server]
        protected override void UpdateServer()
        {
            base.UpdateServer();
            this.InvokeInstanceDevExtMethods(nameof(UpdateServer)); //HOOK
        }

        /// <summary>
    	/// Client-side, throttled update.
    	/// </summary>
        [Client]
        protected override void UpdateClient()
        {

            if (!isLocalPlayer) return;
            if (Tools.AnyInputFocused) return;

            //UPDATE INPUTS - Checks current inputs and updates the attached Movement Profile
            input.UpdateProfile();

            //UPDATE VELOCITY
            agent.velocity = input.motor.GetVelocity(
                new MovementInput(transform.position, transform.rotation, input), input.movement, agent
                //data.verticalMovement, data.horizontalMovement,
                //data.running, data.sneaking,
                //data.strafeLeft, data.strafeRight),
                //data.movement, agent
                );

            base.UpdateClient();
            this.InvokeInstanceDevExtMethods(nameof(UpdateClient)); //HOOK

        }

        /// <summary>
    	/// Client only, late update.
    	/// </summary>
        protected override void LateUpdateClient()
        {
            base.LateUpdateClient();
            this.InvokeInstanceDevExtMethods(nameof(LateUpdateClient)); //HOOK
        }

    }

}