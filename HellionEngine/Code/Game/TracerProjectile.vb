Namespace Game
    Public MustInherit Class TracerProjectile
        Inherits HellionEngine.Physics.SingleRadiusCollisionObject

        Public MuzzleVelocity As Single = 10
        Public Heading As HellionEngine.Support.Vector

        Public DamageValue As Single = 10

        Public Sub New(CollisionRadius As Single, Filepath As String, Heading As HellionEngine.Support.Vector)
            MyBase.New(Filepath, CollisionRadius)

            '  Me.Texture = NewTexture
            ' Me.InternalSprite = New SlimDX.Direct3D9.Sprite(ModuleDevice)
            ' Me.RotationCenter = New BattleZone.Engine.Support.Vector(Me.Texture.Dimensions.X / 2, Me.Texture.Dimensions.Y / 2)
            Me.Rotation.Radians = Math.Atan2(Heading.Y, Heading.X)

            ' Me.PhysicsHandle = BattleZone.Engine.PhysicsSystem.AddHoverObject(Me)

            Me.Heading = Heading

            Me.Mass = 1.0
        End Sub

        Public Overridable Sub OnObjectHit(Other As HellionEngine.Game.HoverTank)
            Other.HitPoints -= Me.DamageValue
        End Sub

        Public Overrides Sub OnPhysicsUpdate(DeltaTimeSeconds As Single)
            MyBase.OnPhysicsUpdate(DeltaTimeSeconds)

            REM Calculate the movement vector for this tank and get two hypothetical positions
            ' Dim CurrentTank As BattleZone.Engine.GameSystem.GameObject = Me.Target

            Dim MovementVector As New HellionEngine.Support.Vector(Me.Heading.X * DeltaTimeSeconds * Me.MuzzleVelocity, Me.Heading.Y * DeltaTimeSeconds * Me.MuzzleVelocity)

            Try
                Dim HypotheticalXTest As New HellionEngine.Support.Vector(Me.Center.X + MovementVector.X, Me.Center.Y)
                Dim HypotheticalYTest As New HellionEngine.Support.Vector(Me.Center.X, MovementVector.Y + Me.Center.Y)

                HellionEngine.Physics.Singleton.SimObjects.IterationBegin(False)
                While (Not HellionEngine.Physics.Singleton.SimObjects.IterationEnd())
                    Dim TestingCollision As HellionEngine.Core.EngineObject = HellionEngine.Physics.Singleton.SimObjects.IterationNext()
                    REM Make sure we're not trying to check against Tracer projectiles, they cause exceptions
                    If (TestingCollision Is Me) Then REM Or TypeOf TestingCollision Is TracerProjectile) Then
                        Continue While
                    End If

                    REM Calculate Distances with our hypothetical positions
                    Dim ObjectDistanceX As Single = Math.Sqrt(Math.Pow(TestingCollision.Center.X - HypotheticalXTest.X, 2) + Math.Pow(TestingCollision.Center.Y - HypotheticalXTest.Y, 2))
                    Dim ObjectDistanceY As Single = Math.Sqrt(Math.Pow(TestingCollision.Center.X - HypotheticalYTest.X, 2) + Math.Pow(TestingCollision.Center.Y - HypotheticalYTest.Y, 2))

                    Dim ObjectDistance As Single = Math.Sqrt(Math.Pow(TestingCollision.Center.X - Me.Center.X, 2) + Math.Pow(TestingCollision.Center.Y - Me.Center.Y, 2))

                    REM Something got hit
                    If (ObjectDistanceX < Me.BoundingCircle Or ObjectDistanceY < Me.BoundingCircle) Then

                        If (TypeOf (TestingCollision) Is HellionEngine.Game.HoverTank) Then
                            Me.OnObjectHit(TestingCollision)
                        End If
                    End If
                End While

            Catch ex As System.Exception
                System.Console.WriteLine(ex.Message)
            End Try

            REM Apply New Position
            Me.Position += MovementVector
        End Sub
    End Class
End Namespace