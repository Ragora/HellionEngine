Namespace Tanks
    Namespace AI
        Public Class AIObjectiveAimAt
            Inherits HellionEngine.Game.HoverTankObjective

            Protected Controlled As HellionEngine.Game.HoverTank = Nothing
            Public TargetLocation As HellionEngine.Support.Vector

            Public Sub New(Controlled As HellionEngine.Game.HoverTank)
                Me.Controlled = Controlled

                Me.TargetLocation = New HellionEngine.Support.Vector(0, 0)
            End Sub

            Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
                MyBase.OnUpdate(DeltaTimeSeconds)

                Me.Controlled.LeftTurning = False
                Me.Controlled.RightTurning = False

                Dim Theta As New HellionEngine.Support.Rotation()

                Dim DistanceX As Single = Math.Abs(Me.Controlled.Position.X - Me.TargetLocation.X)
                Dim DistanceY As Single = Math.Abs(Me.Controlled.Position.Y - Me.TargetLocation.Y)

                Dim Rotation As New HellionEngine.Support.Rotation
                Rotation.Radians = Math.Atan2(DistanceY, DistanceX)

                REM 1st or 4th Quadrant
                If ((Me.TargetLocation.X > Me.Controlled.Position.X And Me.TargetLocation.Y > Me.Controlled.Position.Y) Or (Me.TargetLocation.X < Me.Controlled.Position.X And Me.TargetLocation.Y < Me.Controlled.Position.Y)) Then
                    Theta.Radians = Math.Atan(DistanceY / DistanceX)

                    REM If we're in the fourth quadrant, add PI
                    If (Me.TargetLocation.X < Me.Controlled.Position.X And Me.TargetLocation.Y < Me.Controlled.Position.Y) Then
                        Theta.Radians += Math.PI
                    End If
                ElseIf ((Me.TargetLocation.X < Me.Controlled.Position.X And Me.TargetLocation.Y > Me.Controlled.Position.Y)) Then REM Check 2nd Quadrant
                    Theta.Radians = (Math.PI) - Math.Atan(DistanceY / DistanceX)
                Else
                    Theta.Radians = (Math.PI * 2) - Math.Atan(DistanceY / DistanceX)
                End If

                REM Calculate Difference, really hacky though.
                Dim Difference As Single = Theta.Radians - Me.Controlled.Rotation.Radians

                REM Figure out which way is "faster" to take (basically take the rotation direction that's closer to 2pi or 0)
                Dim DistanceFromZero As Single = Math.Abs(Difference)
                Dim DistanceFrom2Pi As Single = (Math.PI * 2) - Math.Abs(Difference)

                REM Decides which way to turn, which fixes that spinning issue I had at 0 and the bot also now takes the shorter rotation
                If (DistanceFromZero > DistanceFrom2Pi And Difference > 0) Then
                    Me.Controlled.RightTurning = True
                ElseIf (DistanceFromZero > DistanceFrom2Pi And Difference < 0) Then
                    Me.Controlled.LeftTurning = True
                ElseIf (DistanceFromZero < DistanceFrom2Pi And Difference > 0) Then
                    Me.Controlled.LeftTurning = True
                Else
                    Me.Controlled.RightTurning = True
                End If
            End Sub
        End Class

        Public Class AIObjectiveAimAtObject
            Inherits AIObjectiveAimAt

            Public TargetTank As HellionEngine.Game.HoverTank = Nothing

            Public Sub New(Controlled As HellionEngine.Game.HoverTank, Optional TargetTank As HellionEngine.Game.HoverTank = Nothing)
                MyBase.New(Controlled)

                Me.TargetTank = TargetTank
            End Sub

            Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
                If (Not (Me.TargetTank Is Nothing)) Then
                    MyBase.OnUpdate(DeltaTimeSeconds)

                    Me.TargetLocation = Me.TargetTank.Position
                End If
            End Sub
        End Class

        Public Class AIObjectiveEngageObject
            Inherits AIObjectiveAimAtObject

            Public MaximumEngageDistance As Single = 300
            Public MinimumEngageDistance As Single = 250

            Private StrafeTimer As HellionEngine.Support.Timing.TimedEvent

            Public Sub New(Controlled As HellionEngine.Game.HoverTank, Optional TargetTank As HellionEngine.Game.HoverTank = Nothing, Optional MinimumEngageDistance As Single = 250, Optional MaximumEngageDistance As Single = 350)
                MyBase.New(Controlled, TargetTank)

                Me.MaximumEngageDistance = MaximumEngageDistance
                Me.MinimumEngageDistance = MinimumEngageDistance

                Me.StrafeTimerHandle()
            End Sub

            Public Sub StrafeTimerHandle()
                Me.Controlled.LeftwardMoving = False
                Me.Controlled.RightwardMoving = False

                REM Are we within engage distance?
                Dim DistanceFromTarget = Me.Controlled.Center.Distance(Me.TargetTank.Center)
                If (DistanceFromTarget <= Me.MaximumEngageDistance And DistanceFromTarget >= Me.MinimumEngageDistance) Then
                    Dim RandomInteger As Integer = HellionEngine.Support.Random.RandomInteger(0, 10)
                    If (RandomInteger < 3) Then
                        Me.Controlled.LeftwardMoving = True
                    ElseIf (RandomInteger > 7) Then
                        Me.Controlled.RightwardMoving = True
                    End If
                End If

                Me.StrafeTimer = New HellionEngine.Support.Timing.TimedEvent(HellionEngine.Support.Random.RandomSingle(2000, 3000), False)
                AddHandler Me.StrafeTimer.OnFire, AddressOf Me.StrafeTimerHandle
            End Sub

            Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
                If (Not (Me.TargetTank Is Nothing)) Then
                    MyBase.OnUpdate(DeltaTimeSeconds)

                    Me.Controlled.ForwardMoving = False
                    Me.Controlled.BackwardMoving = False
                    Me.Controlled.FiringWeapon = False

                    Dim DistanceFromTarget = Me.Controlled.Center.Distance(Me.TargetTank.Center)
                    If (DistanceFromTarget > Me.MaximumEngageDistance) Then
                        Me.Controlled.ForwardMoving = True
                    ElseIf (DistanceFromTarget < Me.MinimumEngageDistance) Then
                        Me.Controlled.BackwardMoving = True
                    End If

                    REM If Within engage distance, fire weapon
                    If (DistanceFromTarget <= Me.MaximumEngageDistance And DistanceFromTarget >= Me.MinimumEngageDistance) Then
                        Me.Controlled.FiringWeapon = True
                    End If
                End If
            End Sub
        End Class
    End Namespace
End Namespace
