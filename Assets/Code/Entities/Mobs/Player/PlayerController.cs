using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MobController
{
    // Player's melee controller
    [SerializeField] MeleeController playerMelee;
    [SerializeField] protected Camera _camera;
    [SerializeField] bool _isRolling = false;
    [SerializeField] float DetectionDistanceSq;
    [SerializeField] float maxLockOnRadius;
    [SerializeField] HitboxController _playerBullet;
    [SerializeField] float _bulletDamage = 50;
    [SerializeField] Transform _gunSlot;
    [SerializeField] Transform _swordSlot;
    StateManager _stateManager;
    PlayerState _currentState;

    private void Awake()
    {
        base.Awake();
        _stateManager = new StateManager(this, StateManager.State.Idle);
        Entity = this.GetComponent<Player>();
        _camera = Camera.main;
        _swordSlot.gameObject.SetActive(true);
        _gunSlot.gameObject.SetActive(false);
        _animator.SetBool("isMoving", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isPaused) return;
        
        base.Update();

        if(InputManager.GetKeyDown(InputAction.LockOn)) {
            HandleLockOn();
        }

        StateManager.Update();

        if (IsMoving())
            _animator.SetBool("isMoving", true);
        else
            _animator.SetBool("isMoving", false);

        if (!_isRolling) 
        {
            if (InputManager.GetKey(InputAction.Aim))
            {
                _swordSlot.gameObject.SetActive(false);
                _gunSlot.gameObject.SetActive(true);
            }
            else
            {
                _swordSlot.gameObject.SetActive(true);
                _gunSlot.gameObject.SetActive(false);
            }
        }
    }

    // Called by rolling animation
    void AnimationEndRoll()
    {
        _isRolling = false;
    }

    public void LookAtMouse()
    {
        Vector3 P2M;
        if(GetMouseWorldCoords(out P2M)) {
            // Rotate player accordingly
            float targetAngle = Mathf.Atan2(P2M.x, P2M.z) * Mathf.Rad2Deg;
            LookInDirection(targetAngle);
        }
    }

    public bool GetMouseWorldCoords(out Vector3 mouseCoords) {
        // Construct a plane that is level with the player position
        Plane playerPlane = new Plane(Vector3.up, _controller.center);

        // Fire a ray from the mouse screen position into the world
        Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

        if (playerPlane.Raycast(mouseRay, out float distanceToPlane))
        {
            // Calculate hitpoint using ray and distance to plane
            Vector3 mouseHitPoint = mouseRay.GetPoint(distanceToPlane);
            mouseCoords = (mouseHitPoint - transform.position).normalized;
            return true;
        }
        mouseCoords = Vector3.zero;
        return false;
    }

    /**
    Toggle lock on
    */
    public void HandleLockOn() {
        if(IsLockedOn()) {
            LockOn(null);
            return;
        }

        Vector3 mousePos;
        if(!GetMouseWorldCoords(out mousePos)) {
            // TODO: DECIDE ON THIS LockOn(null);
            return;
        }

        // Lock on to something if possible
        // List<Entity> mobs = CurrentRoom.Entities.FindAll((entity) => entity is Mob);

        Mob[] mobs = FindObjectsOfType(typeof(Mob)) as Mob[];
        if(mobs.Length > 1) {
            Mob closestToMouse = null;
            float closestDist = 0f;
            foreach(Mob mob in mobs) {
                if(mob.Equals(this.Mob) || mob.isDead)
                    continue;

                float dist = Mob.DistanceToSq(mob);
                if(closestToMouse == null) {
                    closestToMouse = mob;
                    closestDist = dist;
                    continue;
                }

                if(dist < closestDist) {
                    closestToMouse = mob;
                    closestDist = dist;
                }
            }

            // Matt -- Only did this to get the program working
            // Just wanted to check some changes to the shader
            if (this.CurrentRoom == null)
                LockOn(closestToMouse);
            else if (EntityIsNearby(closestToMouse))
                LockOn(closestToMouse);
        } else // Unlock target
            LockOn(null);     
    }

    private bool EntityIsNearby(Entity o) {
        if (this.Entity.DistanceToSq(o) < DetectionDistanceSq || 
            this.Entity.CurrentRoom == o.CurrentRoom)
            return true;
        return false;
    }

    public override Vector3 CalculateMoveDirection()
    {
        bool iLeft = InputManager.GetKey(InputAction.Left);
        bool iRight = InputManager.GetKey(InputAction.Right);
        bool iBack = InputManager.GetKey(InputAction.Back);
        bool iForward = InputManager.GetKey(InputAction.Forward);
        bool isMoving = iForward || iRight || iBack || iLeft;

        // Prevents random movement drift due to floating point stuff
        if(!isMoving) {
            return new Vector3(0,0,0);
        }

        float left = iLeft ? -1.0f : 0;
        float right = iRight ? 1.0f : 0;
        float forward = iForward ? 1.0f : 0;
        float back = iBack ? -1.0f : 0;
        float horizontal = left + right;
        float vertical = forward + back;
        
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        // Calculate the correct movement angle relative to the Camera (Degrees)
        float moveAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, moveAngle, 0f) * Vector3.forward;
        return moveDir;
    }

    // Getters and Setters
    public Camera Camera { get { return this._camera; } }
    public bool IsRolling { get => _isRolling; set => _isRolling = value; }
    public MeleeController PlayerMelee { get => playerMelee; set => playerMelee = value; }
    public StateManager StateManager { get => _stateManager; set => _stateManager = value; }

    public Vector3 Velocity { get => Motion.Velocity; set => Motion.Velocity = value; }
    public HitboxController PlayerBullet { get => _playerBullet; set => _playerBullet = value; }
    public float BulletDamage { get => _bulletDamage; set => _bulletDamage = value; }
    public Transform GunSlot { get => _gunSlot; set => _gunSlot = value; }
    public Transform SwordSlot { get => _swordSlot; set => _swordSlot = value; }
}
