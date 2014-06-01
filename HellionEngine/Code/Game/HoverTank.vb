Namespace Game
    Public MustInherit Class HoverTankObjective
        Inherits HellionEngine.Core.EngineObject

        Public Sub New()
            MyBase.New()


        End Sub
    End Class

    Public MustInherit Class HoverTank
        Inherits HellionEngine.Game.HoverObject

        REM How fast the tank accelerates to move forward in pixels per second
        Public ForwardAcceleration As Single = 30
        REM How much force is applied to move backward
        Public BackwardAcceleration As Single = 10
        REM How much force is applied to strafe
        Public StrafingAcceleration As Single = 26
        REM Turning Speed
        Public TurningSpeed As Single = (Math.PI * 2) / 2
        REM The Stabilizers are what eventually bring your hover tank in a space environment to a halt.
        Public MovementStabilizersStrength As Single = 12
        REM Stabilizers applied for user-initiated "braking".
        Public BrakingStabilizersStrength As Single = 20
        REM Our maximum turning speed (this is the threshold where user input cannot go any further in either direction)
        Public MaximumTurningSpeed As Single = (Math.PI * 2) / 2
        REM Our maximum movement speed
        Public MaximumMovingSpeed = 4
        REM The constant equalizer that attempts to take our tank rotation speed back to zero
        Public TurningStabilizerStrength As Single = (Math.PI * 2) / 4

        Public HitPoints As Single = 100

        REM Tank State Variables
        Public ForwardMoving As Boolean = False
        Public BackwardMoving As Boolean = False
        Public LeftwardMoving As Boolean = False
        Public RightwardMoving As Boolean = False
        Public LeftTurning As Boolean = False
        Public RightTurning As Boolean = False
        Public Braking As Boolean = False

        Public Objective As HoverTankObjective = Nothing

        Public FiringWeapon As Boolean = False

        Public Sub New(Filepath As String)
            MyBase.New(5, Filepath)

            Me.Mass = 1.0
        End Sub

        Public Overrides Sub OnPhysicsUpdate(DeltaTimeSeconds As Single)
            MyBase.OnPhysicsUpdate(DeltaTimeSeconds)

            MyBase.OnUpdate(DeltaTimeSeconds)

            If (Me.LeftTurning And Me.RotationVelocity < Me.MaximumTurningSpeed) Then
                ' If (Me.TurningSpeed + Me.RotationVelocity > Me.MaximumTurningSpeed) Then
                '  Me.RotationVelocity = Me.MaximumTurningSpeed
                '  Return
                '  End If

                Me.RotationVelocity += Me.TurningSpeed * DeltaTimeSeconds
            End If

            If (Me.RightTurning And Me.RotationVelocity > -Me.MaximumTurningSpeed) Then
                ' If (Me.TurningSpeed + Me.RotationVelocity > Me.MaximumTurningSpeed) Then
                'Me.RotationVelocity = Me.MaximumTurningSpeed
                '   Return
                '  End If

                Me.RotationVelocity -= Me.TurningSpeed * DeltaTimeSeconds
            End If

            REM Check Forward and Back Movement
            If (Me.ForwardMoving) Then
                Me.Velocity += Me.ForwardVector * Me.ForwardAcceleration * DeltaTimeSeconds
            End If

            If (Me.BackwardMoving) Then
                Me.Velocity -= Me.ForwardVector * Me.ForwardAcceleration * DeltaTimeSeconds
            End If

            REM Check Sideways movement
            If (Me.RightwardMoving) Then
                Me.Velocity += Me.RightVector * Me.StrafingAcceleration * DeltaTimeSeconds
            End If

            If (Me.LeftwardMoving) Then
                Me.Velocity += Me.LeftVector * Me.StrafingAcceleration * DeltaTimeSeconds
            End If

            REM Check Movement Stabilizers
            Dim NewVelocityX As Single = Me.Velocity.X
            Dim NewVelocityY As Single = Me.Velocity.Y

            Dim Modifier As Single = 0
            If (NewVelocityX < 0) Then
                Modifier = Me.MovementStabilizersStrength * DeltaTimeSeconds

                Dim VelocityXAbs As Single = Math.Abs(NewVelocityX)
                If (Modifier > VelocityXAbs) Then
                    Modifier -= VelocityXAbs
                End If

                NewVelocityX += Modifier
            Else
                Modifier = Me.MovementStabilizersStrength * DeltaTimeSeconds

                If (Modifier > NewVelocityX) Then
                    Modifier -= NewVelocityX
                End If

                NewVelocityX -= Modifier
            End If

            If (NewVelocityY < 0) Then
                Modifier = Me.MovementStabilizersStrength * DeltaTimeSeconds

                Dim VelocityYAbs As Single = Math.Abs(NewVelocityY)
                If (Modifier > VelocityYAbs) Then
                    Modifier -= VelocityYAbs
                End If

                NewVelocityY += Modifier
            Else
                Modifier = Me.MovementStabilizersStrength * DeltaTimeSeconds

                If (Modifier > NewVelocityY) Then
                    Modifier -= NewVelocityY
                End If

                NewVelocityY -= Modifier
            End If

            REM Check Braking Stabilizers
            If (Me.Braking) Then
                If (NewVelocityX < 0) Then
                    Modifier = Me.BrakingStabilizersStrength * DeltaTimeSeconds

                    Dim VelocityXAbs As Single = Math.Abs(NewVelocityX)
                    If (Modifier > VelocityXAbs) Then
                        Modifier -= VelocityXAbs
                    End If

                    NewVelocityX += Modifier
                Else
                    Modifier = Me.BrakingStabilizersStrength * DeltaTimeSeconds

                    If (Modifier > NewVelocityX) Then
                        Modifier -= NewVelocityX
                    End If

                    NewVelocityX -= Modifier
                End If

                If (NewVelocityY < 0) Then
                    Modifier = Me.BrakingStabilizersStrength * DeltaTimeSeconds

                    Dim VelocityYAbs As Single = Math.Abs(NewVelocityY)
                    If (Modifier > VelocityYAbs) Then
                        Modifier -= VelocityYAbs
                    End If

                    NewVelocityY += Modifier
                Else

                    Modifier = Me.BrakingStabilizersStrength * DeltaTimeSeconds

                    If (Modifier > NewVelocityY) Then
                        Modifier -= NewVelocityY
                    End If

                    NewVelocityY -= Modifier
                End If
            End If

            Me.Velocity = New HellionEngine.Support.Vector(NewVelocityX, NewVelocityY)

            REM Check turning stabilizers
            If (Me.RotationVelocity < 0 And Me.RotationVelocity <> 0) Then
                Modifier = Me.TurningStabilizerStrength * DeltaTimeSeconds

                Dim RotationVelocityAbs As Single = Math.Abs(Me.RotationVelocity)
                If (Modifier > RotationVelocityAbs) Then
                    Modifier -= RotationVelocityAbs
                End If

                If (Modifier < 0.02) Then
                    Modifier = 0
                    Me.RotationVelocity = 0
                End If

                Me.RotationVelocity += Modifier
            ElseIf (Me.RotationVelocity <> 0) Then
                Modifier = Me.TurningStabilizerStrength * DeltaTimeSeconds

                If (Modifier > Me.RotationVelocity) Then
                    Modifier -= Me.RotationVelocity
                End If

                If (Modifier < 0.02) Then
                    Modifier = 0
                    Me.RotationVelocity = 0
                End If

                Me.RotationVelocity -= Modifier
            End If

            REM Sim weapon fire
            If (Me.FiringWeapon) Then
                '  Dim Projectile As New BattleZone.Game.Drawables.SharedObjects.SPStabber(Me.ForwardVector)
                '   Projectile.Position.CopyFrom(Me.Position)
            End If

            If (Not (Me.Objective Is Nothing)) Then
                Me.Objective.OnUpdate(DeltaTimeSeconds)
            End If
        End Sub
    End Class
End Namespace