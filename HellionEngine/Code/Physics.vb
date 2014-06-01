Public Module Physics
    Private SingletonInstance As ClassInstance

    Public ReadOnly Property Singleton() As ClassInstance
        Get
            Return SingletonInstance
        End Get
    End Property

    Public Function Instantiate() As ClassInstance
        If (SingletonInstance Is Nothing) Then
            SingletonInstance = New ClassInstance()
        End If

        Return SingletonInstance
    End Function

    Public Class ClassInstance
        Inherits HellionEngine.Core.EngineObject

        Public SimObjects As HellionEngine.Support.DynamicArray

        Public Sub New()
            Me.SimObjects = New HellionEngine.Support.DynamicArray(0)
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.SimObjects.IterationBegin(False)
            While (Not Me.SimObjects.IterationEnd())
                Me.SimObjects.IterationNext().Dispose()
            End While

            Me.SimObjects.Dispose()
            Me.SimObjects = Nothing
            SingletonInstance = Nothing
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            For Iteration As Integer = 0 To Me.SimObjects.Length() - 1
                REM God...
                Dim SimObject As HellionEngine.Core.EngineObject = Me.SimObjects.GetIndex(Iteration)

                If (SimObject.Disposed()) Then
                    Me.SimObjects.RemoveObject(SimObject)
                    Exit For
                Else
                    SimObject.OnPhysicsUpdate(DeltaTimeSeconds)
                End If
            Next

            'Me.SimObjects.IterationBegin(False)
            'While (Not Me.SimObjects.IterationEnd())
            'Me.SimObjects.IterationNext().OnPhysicsUpdate(DeltaTimeSeconds)
            ' End While
        End Sub
    End Class

    Public Interface PhysicalObject
        Inherits HellionEngine.Core.DisposableObject

        Property Velocity As HellionEngine.Support.Vector
        Property RotationVelocity As Single
        Property Mass As Single

        Sub OnCollision(Other As PhysicalObject)
        Sub OnUpdate(DeltaTimeSeconds As Single)
        Function TestCollision(Other As HellionEngine.Core.EngineObject) As Boolean
    End Interface

    Public MustInherit Class SingleRadiusCollisionObject
        Inherits HellionEngine.Core.EngineObject

        Public CollisionRadius As Single

        Private InternalSprite As SlimDX.Direct3D9.Sprite
        Private InternalTexture As HellionEngine.Rendering.Texture

        Public Sub New(Filepath As String, CollisionRadius As Single)
            MyBase.New()

            Me.CollisionRadius = CollisionRadius

            Me.InternalSprite = New SlimDX.Direct3D9.Sprite(HellionEngine.Rendering.DeviceInstance)
            Me.InternalTexture = HellionEngine.Rendering.Singleton.GetTexture(Filepath)
            Me.RotationCenter = New HellionEngine.Support.Vector(Me.Dimensions.X / 2, Me.Dimensions.Y / 2)

            ' Me.PhysicsHandle = BattleZone.Engine.PhysicsSystem.AddHoverObject(Me)

            Me.Mass = 1.0

            HellionEngine.Physics.Singleton.SimObjects.Append(Me)
        End Sub

        Public Overrides Sub OnDraw()
            MyBase.OnDraw()

            If (Not Me.Visible Or Me.Disposed() Or Me.InternalTexture Is Nothing) Then
                Return
            End If

            REM Stupid code...
            If (Me.InternalTexture.Disposed()) Then
                Me.InternalTexture = HellionEngine.Rendering.Singleton.GetTexture(Me.InternalTexture.Filepath)

                If (Me.InternalTexture Is Nothing) Then
                    Return
                End If
            End If

            Me.InternalSprite.Begin(SlimDX.Direct3D9.SpriteFlags.AlphaBlend Or SlimDX.Direct3D9.SpriteFlags.ObjectSpace)

            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Identity)
            HellionEngine.Rendering.DeviceInstance.SetTransform(SlimDX.Direct3D9.TransformState.World, SlimDX.Matrix.Transformation2D(New SlimDX.Vector2(0, 0), 0, Me.Scale.SlimDX, Me.RotationCenter.SlimDX, Me.Rotation.Radians, Me.Position.SlimDX))

            Me.InternalSprite.Draw(Me.InternalTexture.RenderTexture, Me.Color.DirectXColor)
            Me.InternalSprite.End()
        End Sub

        Public Overrides Property Dimensions() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Me.Scale.X * Me.InternalTexture.Dimensions.X, Me.Scale.Y * Me.InternalTexture.Dimensions.Y)
            End Get
            Set(value As HellionEngine.Support.Vector)
                Me.Scale = New HellionEngine.Support.Vector(value.X / Me.InternalTexture.Dimensions.X, value.Y / Me.InternalTexture.Dimensions.Y)
            End Set
        End Property

        Public Overrides Sub OnCollision(Other As PhysicalObject)

        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.InternalTexture.Dispose()
            Me.InternalSprite.Dispose()
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)

        End Sub

        Public Overrides Function TestCollision(Other As HellionEngine.Core.EngineObject) As Boolean
            Dim DistanceFromOther As Single = Math.Sqrt(Math.Pow(Me.Position.X - Other.Position.X, 2) + Math.Pow(Me.Position.Y - Other.Position.Y, 2))

            If (DistanceFromOther <= Me.CollisionRadius) Then
                Return True
            End If

            Return False
        End Function
    End Class
End Module

