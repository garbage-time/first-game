using Godot;
using System;

public partial class player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public const float Acceleration = 100.0f;
	public float MaxSpeed = 1000.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private int jumpCount = 0;
	private const int maxJumpCount = 2;

	private AnimationPlayer ap;
	private Sprite2D sprite;

	public bool isMovingLeft = Input.IsActionPressed("ui_left");
	public bool isMovingRight = Input.IsActionPressed("ui_right");


	public override void _Ready()
	{
		ap = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode
		<Sprite2D>("Sprite2D");
	}

	private void HandleAnimations(Vector2 direction)
	{
		if (direction == Vector2.Zero)
		{
			ap.Play("idle");
		}
		if (direction != Vector2.Zero && (isMovingLeft || isMovingRight))
		{
			ap.Play("_run");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;
		if (velocity.Y > 5000)
			velocity.Y = 5000;

		// Reset jumpCount if player is on the floor.
		if (IsOnFloor())
		{
			jumpCount = 0;
			// Decelerate the player's speed faster when on the ground.
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * 3 * (float)delta);
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (IsOnFloor() || jumpCount < maxJumpCount)
			{
				velocity.Y = JumpVelocity;
				jumpCount++;
			}
		}
		isMovingLeft = Input.IsActionPressed("ui_left");
		isMovingRight = Input.IsActionPressed("ui_right");

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		HandleAnimations(direction);
		if (direction.Length() > 1)
		{
			direction = direction.Normalized(); // Normalize the vector if its length is greater than 1.
		}
		if (direction != Vector2.Zero)
		{	
			sprite.FlipH = direction.X < 0;
			velocity.X = Speed * direction.X;
			velocity.X = Mathf.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * (float)delta);
		}
		// Move the player.
		GD.Print(ap.CurrentAnimation == "idle");

		Velocity = velocity;
		MoveAndSlide();
	}
}
