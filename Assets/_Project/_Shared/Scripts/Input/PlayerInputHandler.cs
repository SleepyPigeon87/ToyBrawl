using UnityEngine;
using UnityEngine.InputSystem;

namespace Brawler.Input
{
    /// <summary>
    /// Handles input for a single player in the brawler.
    /// Each fighter has their own PlayerInputHandler with a specific playerIndex.
    ///
    /// For 2-player local:
    ///   - Player 0: WASD + Space/J/K or Gamepad 1
    ///   - Player 1: Arrows + RCtrl/Numpad1/2 or Gamepad 2
    ///
    /// The InputActionAsset should have separate control schemes for each player.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Player Settings")]
        [Tooltip("Which player this handler is for (0 = Player 1, 1 = Player 2)")]
        [SerializeField] private int playerIndex = 0;

        [Header("Input Asset")]
        [Tooltip("The InputActionAsset containing the Player action map.")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Config")]
        [Tooltip("Input configuration for deadzones and buffering.")]
        [SerializeField] private InputConfig config;

        // Processed input state (read by FighterMovement/AttackController)
        public Vector2 MoveInput { get; private set; }
        public bool JumpBuffered => jumpBufferTimer > 0f;
        public bool JumpHeld { get; private set; }
        //public bool DashBuffered => dashBufferTimer > 0f;
        public bool AttackBuffered => attackBufferTimer > 0f;
        public bool SpecialBuffered => specialBufferTimer > 0f;
        public bool UsingGamepad { get; private set; }
        public int PlayerIndex => playerIndex;

        // Input action references
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction lightAttackAction;
        private InputAction ultimateAction;
        private InputAction grabTossAction;
        private InputAction dodgeRollAction;

        // Buffer timers
        private float jumpBufferTimer;
        //private float dashBufferTimer;
        private float attackBufferTimer;
        private float ultimateBufferTimer;

        // Raw input for debugging
        private Vector2 rawMoveInput;

        /// <summary>
        /// Initialize the input handler for a specific player.
        /// Call this after instantiating the fighter prefab.
        /// </summary>
        public void Initialize(int index)
        {
            Debug.Log($"[PlayerInputHandler] Initialize called! Changing my index from {playerIndex} to {index} on GameObject: {gameObject.name}");
            // Clean up previous actions if re-initializing after Awake
            DisableInputActions();

            playerIndex = index;
            SetupInputActions();
            EnableInputActions();
        }

        private void Awake()
        {
            // Clone the asset so each player gets independent action state.
            // Without this, two handlers sharing the same asset conflict on Enable.
            if (inputActions != null)
            {
                inputActions = Instantiate(inputActions);
            }

            // If not initialized externally, set up with serialized values
            if (moveAction == null)
            {
                SetupInputActions();
            }
        }

        private void OnDestroy()
        {
            CleanupInputActions();

            // Destroy the cloned asset
            if (inputActions != null)
            {
                Destroy(inputActions);
            }
        }

        private void OnEnable()
        {
            if (moveAction != null)
            {
                EnableInputActions();
            }
        }

        private void OnDisable()
        {
            DisableInputActions();
        }

        private void Update()
        {
            ProcessMoveInput();
            UpdateBufferTimers();
        }

        private void SetupInputActions()
        {
            if (inputActions == null)
            {
                Debug.LogError($"[PlayerInputHandler P{playerIndex}] InputActionAsset not assigned!", this);
                return;
            }

            // Find the action map - we use Player1 or Player2 based on index
            string mapName = playerIndex == 0 ? "Player1" : "Player2";
            var playerMap = inputActions.FindActionMap(mapName);

            // Fallback to generic "Player" map if specific maps don't exist
            if (playerMap == null)
            {
                playerMap = inputActions.FindActionMap("Player");
                if (playerMap == null)
                {
                    Debug.LogError($"[PlayerInputHandler P{playerIndex}] No '{mapName}' or 'Player' action map found!", this);
                    return;
                }
            }

            moveAction = playerMap.FindAction("Move");
            jumpAction = playerMap.FindAction("Jump");
            lightAttackAction = playerMap.FindAction("LightAttack");
            ultimateAction = playerMap.FindAction("Ultimate");
            grabTossAction = playerMap.FindAction("GrabToss");
            dodgeRollAction = playerMap.FindAction("DodgeRoll");

            if (moveAction == null)
                Debug.LogError($"[PlayerInputHandler P{playerIndex}] 'Move' action not found!", this);
            if (jumpAction == null)
                Debug.LogError($"[PlayerInputHandler P{playerIndex}] 'Jump' action not found!", this);
            if (lightattackAction == null)
                Debug.LogWarning($"[PlayerInputHandler P{playerIndex}] 'Attack' action not found. Add it for combat.", this);
        }

        private void EnableInputActions()
        {
            if (moveAction == null) return;

            moveAction.Enable();
            jumpAction?.Enable();
            ultimateAction?.Enable();
            lightAttackAction?.Enable();
            dodgeRollAction?.Enable();
            grabTossAction?.Enable();

            // Subscribe to button events
            if (jumpAction != null)
            {
                jumpAction.performed += OnJumpPerformed;
                jumpAction.canceled += OnJumpCanceled;
            }

           /* if (dashAction != null)
            {
                dashAction.performed += OnDashPerformed;
            }*/

            if (lightAttackAction != null)
            {
                lightAttackAction.performed += OnAttackPerformed;
            }

            if (ultimateAction != null)
            {
                ultimateAction.performed += OnUpdatePerformed;
            }

            if (dodgeRollAction != null) {
                dodgeRollAction.performed += OnDodgeRollPerformed;
            }

            if (grabTossAction != null) {
                grabTossAction.performed += OnGrabTossPerformed;
            }


            InputSystem.onActionChange += OnActionChange;
        }

        private void DisableInputActions()
        {
            if (moveAction == null) return;

            if (jumpAction != null)
            {
                jumpAction.performed -= OnJumpPerformed;
                jumpAction.canceled -= OnJumpCanceled;
            }

            if (dashAction != null)
            {
                dashAction.performed -= OnDashPerformed;
            }

            if (attackAction != null)
            {
                attackAction.performed -= OnAttackPerformed;
            }

            if (specialAction != null)
            {
                specialAction.performed -= OnSpecialPerformed;
            }

            InputSystem.onActionChange -= OnActionChange;

            moveAction?.Disable();
            jumpAction?.Disable();
            dashAction?.Disable();
            attackAction?.Disable();
            specialAction?.Disable();
        }

        private void CleanupInputActions()
        {
            DisableInputActions();
        }

        private void ProcessMoveInput()
        {
            if (moveAction == null) return;

            rawMoveInput = moveAction.ReadValue<Vector2>();
            MoveInput = ApplyDeadzone(rawMoveInput);
        }

        private Vector2 ApplyDeadzone(Vector2 input)
        {
            float magnitude = input.magnitude;
            float deadzone = config != null ? config.deadzone : 0.15f;

            if (magnitude < deadzone)
            {
                return Vector2.zero;
            }

            float rescaledMagnitude = (magnitude - deadzone) / (1f - deadzone);
            rescaledMagnitude = Mathf.Clamp01(rescaledMagnitude);
            return input.normalized * rescaledMagnitude;
        }

        // Jump
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpHeld = true;
            float bufferDuration = config != null ? config.jumpBufferDuration : 0.1f;
            jumpBufferTimer = bufferDuration;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            JumpHeld = false;
        }

        public void ConsumeJumpBuffer()
        {
            jumpBufferTimer = 0f;
        }

        // Dash
        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            float bufferDuration = config != null ? config.dashBufferDuration : 0.08f;
            dashBufferTimer = bufferDuration;
        }

        public void ConsumeDashBuffer()
        {
            dashBufferTimer = 0f;
        }

        // Attack
        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            float bufferDuration = config != null ? config.attackBufferDuration : 0.1f;
            attackBufferTimer = bufferDuration;
        }

        public void ConsumeAttackBuffer()
        {
            attackBufferTimer = 0f;
        }

        // Special
        private void OnSpecialPerformed(InputAction.CallbackContext context)
        {
            float bufferDuration = config != null ? config.attackBufferDuration : 0.1f;
            specialBufferTimer = bufferDuration;
        }

        public void ConsumeSpecialBuffer()
        {
            specialBufferTimer = 0f;
        }

        private void UpdateBufferTimers()
        {
            if (jumpBufferTimer > 0f)
                jumpBufferTimer -= Time.deltaTime;

            if (dashBufferTimer > 0f)
                dashBufferTimer -= Time.deltaTime;

            if (attackBufferTimer > 0f)
                attackBufferTimer -= Time.deltaTime;

            if (specialBufferTimer > 0f)
                specialBufferTimer -= Time.deltaTime;
        }

        private void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;

            var action = obj as InputAction;
            if (action == null) return;

            // Only track device for actions belonging to this player
            if (action.actionMap?.name != (playerIndex == 0 ? "Player1" : "Player2") &&
                action.actionMap?.name != "Player")
            {
                return;
            }

            var device = action.activeControl?.device;
            if (device != null)
            {
                UsingGamepad = device is Gamepad;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 pos = transform.position + Vector3.up * 2f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + new Vector3(rawMoveInput.x, rawMoveInput.y, 0));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + new Vector3(MoveInput.x, MoveInput.y, 0));

            Gizmos.color = Color.yellow;
            float deadzone = config != null ? config.deadzone : 0.15f;
            DrawGizmoCircle(pos, deadzone, 16);
        }

        private void DrawGizmoCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
