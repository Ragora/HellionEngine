Namespace Game
    Public MustInherit Class HoverObject
        Inherits HellionEngine.Physics.SingleRadiusCollisionObject

        Public Sub New(CollisionRadius As Single, Filepath As String)
            MyBase.New(Filepath, CollisionRadius)

            '  Me.Texture = NewTexture
            ' Me.InternalSprite = New SlimDX.Direct3D9.Sprite(ModuleDevice)
            ' Me.RotationCenter = New BattleZone.Engine.Support.Vector(Me.Texture.Dimensions.X / 2, Me.Texture.Dimensions.Y / 2)

            ' Me.PhysicsHandle = BattleZone.Engine.PhysicsSystem.AddHoverObject(Me)

            Me.Mass = 1.0
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            REM Calculate the movement vector for this tank and get two hypothetical positions
            ' Dim CurrentTank As BattleZone.Engine.GameSystem.GameObject = Me.Target

            Dim MovementVector As New HellionEngine.Support.Vector(Me.Velocity.X * DeltaTimeSeconds, Me.Velocity.Y * DeltaTimeSeconds)

            Try
                Dim HypotheticalXTest As New HellionEngine.Support.Vector(Me.Center.X + MovementVector.X, Me.Center.Y)
                Dim HypotheticalYTest As New HellionEngine.Support.Vector(Me.Center.X, MovementVector.Y + Me.Center.Y)

                HellionEngine.Physics.Singleton.SimObjects.IterationBegin(False)
                While (Not HellionEngine.Physics.Singleton.SimObjects.IterationEnd())
                    Dim TestingCollision As HellionEngine.Core.EngineObject = HellionEngine.Physics.Singleton.SimObjects.IterationNext()
                    REM Make sure we're not trying to check against Tracer projectiles, they cause exceptions
                    If (TestingCollision Is Me Or TypeOf (TestingCollision) Is HellionEngine.Game.TracerProjectile) Then REM Or TypeOf TestingCollision Is TracerProjectile) Then
                        Continue While
                    End If

                    REM Calculate Distances with our hypothetical positions
                    Dim ObjectDistanceX As Single = Math.Sqrt(Math.Pow(TestingCollision.Center.X - HypotheticalXTest.X, 2) + Math.Pow(TestingCollision.Center.Y - HypotheticalXTest.Y, 2))
                    Dim ObjectDistanceY As Single = Math.Sqrt(Math.Pow(TestingCollision.Center.X - HypotheticalYTest.X, 2) + Math.Pow(TestingCollision.Center.Y - HypotheticalYTest.Y, 2))

                    Dim ObjectDistance As Single = Math.Sqrt(Math.Pow(TestingCollision.Center.X - Me.Center.X, 2) + Math.Pow(TestingCollision.Center.Y - Me.Center.Y, 2))

                    REM Check Collisions
                    Dim Collided As Boolean = False
                    If (ObjectDistanceX < Me.BoundingCircle) Then
                        Collided = True
                        MovementVector.X = 0
                    End If

                    If (ObjectDistanceY < Me.BoundingCircle) Then
                        Collided = True
                        MovementVector.Y = 0
                    End If

                    REM Okay... We got a collision, set the position back somewhat to start with
                    If (Collided) Then
                        REM Calculate Total MAss
                        Dim TotalMass As Single = Me.Mass + TestingCollision.Mass
                        Dim CurrentHandleMassPercentage As Single = Me.Mass / TotalMass
                        Dim TestingCollisionMassPercentage As Single = TestingCollision.Mass / TotalMass

                        REM Calculate the velocity pool
                        Dim SharedVelocity As Single = (Me.Velocity + TestingCollision.Velocity).Length()

                        Dim TowardsTestingCollision As HellionEngine.Support.Vector = Me.Center - TestingCollision.Center
                        TowardsTestingCollision = TowardsTestingCollision.Normalize()

                        Me.Velocity = New HellionEngine.Support.Vector(SharedVelocity * TestingCollisionMassPercentage, SharedVelocity * TestingCollisionMassPercentage) * TowardsTestingCollision
                        TestingCollision.Velocity = New HellionEngine.Support.Vector(SharedVelocity * CurrentHandleMassPercentage, SharedVelocity * CurrentHandleMassPercentage) * -TowardsTestingCollision
                    End If
                End While

            Catch ex As System.Exception
                System.Console.WriteLine(ex.Message)
            End Try

            REM Apply New Position
            Me.Position += MovementVector

            REM Apply New Rotation
            Me.Rotation.Radians += Me.RotationVelocity * DeltaTimeSeconds
        End Sub
    End Class
End Namespace