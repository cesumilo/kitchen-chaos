using Godot;
using Godot.Collections;

public partial class Player : CharacterBody3D, IKitchenObjectHolder
{
    // Members
    [Export] float speed = 300;
    [Export] float gravity = 50;
    [Export] float jumpForce = 10;
    [Export] float interactDistance = 2.5f;
    [Export] float rotationSpeed = 0.4f;
    Vector3 direction = Vector3.ModelFront;
    Vector3 targetDirection = Vector3.ModelFront;
    float currentAngle = 0;
    Basis currentBasis = Basis.Identity;
    AnimationTree animator;
    Interactable selectedInteractable;
    Marker3D kitchenObjectSpanwer;
    KitchenObject objectHolder;

    // Signals
    public partial class OnIteractableSelectedParams : Node
    {
        public GodotObject selected;
    }
    [Signal] public delegate void OnIteractableSelectedEventHandler(OnIteractableSelectedParams args);

    public override void _Ready()
    {
        animator = GetNode<AnimationTree>("AnimationPlayer/AnimationTree");
        kitchenObjectSpanwer = GetNode<Marker3D>("KitchenObjectPosition");
    }

    Vector3 ApplyGravity(Vector3 moveVect, float delta)
    {
        if (!IsOnFloor())
        {
            return moveVect + Vector3.Down * gravity * delta;
        }
        return moveVect;
    }

    Vector3 ApplyJumpForce(Vector3 moveVect)
    {
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            return moveVect + Vector3.Up * jumpForce;
        }
        return moveVect;
    }

    void RotatePlayerFromCurrentBasis(float angle)
    {
        currentAngle = Mathf.LerpAngle(currentAngle, angle, Mathf.Clamp(rotationSpeed, 0.1f, 1f));

        Transform3D transform = Transform;
        transform.Basis = currentBasis;
        Transform = transform;

        RotateY(currentAngle);

        if (Utils.MoreOrLessEqual(currentAngle, angle, 0.00001f))
        {
            SetReachedDirection();
        }
    }

    void SetReachedDirection()
    {
        direction = targetDirection;
        currentBasis = Transform.Basis;
        currentAngle = 0;
    }

    void ResetCurrentBasisAndAngle()
    {
        currentBasis = Transform.Basis;
        direction = direction.Rotated(Vector3.Up, currentAngle);
        currentAngle = 0;
    }

    void OrientPlayer(Vector3 moveVect)
    {
        Vector3 hDir = new(moveVect.X, 0f, moveVect.Z);

        if (hDir != Vector3.Zero)
        {
            if (hDir != targetDirection)
            {
                ResetCurrentBasisAndAngle();
            }

            float angle = Utils.OrientedAngleBetween(hDir, direction, Vector3.Up);
            targetDirection = hDir;

            if (angle != 0)
            {
                RotatePlayerFromCurrentBasis(angle);
            }
            else
            {
                SetReachedDirection();
            }
        }
        else
        {
            float angle = Utils.OrientedAngleBetween(targetDirection, direction, Vector3.Up);

            if (angle == 0)
            {
                SetReachedDirection();
            }
            else
            {
                RotatePlayerFromCurrentBasis(angle);
            }
        }
    }

    void UpdateAnimationTreeConditions()
    {
        animator.Set("parameters/conditions/idle", Velocity == Vector3.Zero);
        animator.Set("parameters/conditions/walking", !(Velocity == Vector3.Zero));
    }

    public override void _Process(double delta)
    {
        UpdateAnimationTreeConditions();
        HandleInteractions();
    }

    void SetSelectedInteractable(GodotObject obj)
    {
        if (obj != selectedInteractable)
        {
            EmitSignal(SignalName.OnIteractableSelected, new OnIteractableSelectedParams() { selected = obj });
        }
        selectedInteractable = obj != null ? obj as Interactable : null;
    }

    void HandleInteractions()
    {
        Vector3 dir = new(direction.X, 1, direction.Z);
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(Position, Position + dir.Normalized() * interactDistance, CollisionMask);
        Dictionary result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            Variant collider = result["collider"];
            switch (collider.VariantType)
            {
                case Variant.Type.Object:
                    {
                        GodotObject obj = collider.AsGodotObject();
                        if (obj is Interactable)
                        {
                            // Select interactable
                            SetSelectedInteractable(obj);
                        }
                        else
                        {
                            SetSelectedInteractable(null);
                        }
                        break;
                    }
                default:
                    {
                        SetSelectedInteractable(null);
                        break;
                    }
            }
        }
        else
        {
            SetSelectedInteractable(null);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputVect = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Vector3 moveVect = new(inputVect.X * speed * (float)delta, Velocity.Y, inputVect.Y * speed * (float)delta);

        // If newMoveVect != currentMoveVect
        // . Change player orientation
        // Else
        // . Continue interpolation

        OrientPlayer(moveVect);

        moveVect = ApplyGravity(moveVect, (float)delta);
        moveVect = ApplyJumpForce(moveVect);
        Velocity = moveVect;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            selectedInteractable?.Interact(this);
        }
    }

    public Marker3D GetKitchenObjectSpawn()
    {
        return kitchenObjectSpanwer;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        objectHolder = kitchenObject;
        objectHolder.SetHolder(this);
        kitchenObjectSpanwer.AddChild(objectHolder);
    }

    public KitchenObject GetKitchenObject()
    {
        return objectHolder;
    }

    public void ClearKitchenObject()
    {
        if (objectHolder != null)
        {
            kitchenObjectSpanwer.RemoveChild(objectHolder);
        }
        objectHolder = null;
    }

    public bool IsHoldingKitchenObject()
    {
        return objectHolder != null;
    }
}
