Namespace Core
    Public Module EngineObjectInternal
        Public EngineObjectInstances As HellionEngine.Support.DynamicArray

        Public Sub Cleanup()
#If DEBUG Then
            HellionEngine.Logging.Write("EngineObject.vb [EngineObjectInternal-Cleanup]: Checking for live EngineObject instances ...")
#End If

            Dim LiveEngineObjectCount As Integer = 0
            EngineObjectInstances.IterationBegin(False)
            While (Not EngineObjectInstances.IterationEnd())
                Dim EngineObjectRef As HellionEngine.Core.EngineObject = EngineObjectInstances.IterationNext()

                If (Not EngineObjectRef.Disposed()) Then
#If DEBUG Then
                    HellionEngine.Logging.Write("EngineObject.vb [EngineObjectInternal-Cleanup]: Found '" + EngineObjectRef.ToString() + "'")
#End If
                    EngineObjectRef.Dispose()
                    LiveEngineObjectCount += 1
                End If
            End While

#If DEBUG Then
            HellionEngine.Logging.Write("EngineObject.vb [EngineObjectInternal-Cleanup]: Found " + LiveEngineObjectCount.ToString() + " total live EngineObject instances.")
#End If
        End Sub
    End Module

    Public Class EngineObject
        Implements HellionEngine.Physics.PhysicalObject
        Implements HellionEngine.Rendering.RenderedObject
        Implements HellionEngine.Interaction.InteractiveObject

        Private InternalDisposed As Boolean
        Private InternalRenderLayer As Integer

        Public Sub New()
            Me.InternalDisposed = False

            Me.InternalRenderLayer = -1

            If (EngineObjectInternal.EngineObjectInstances Is Nothing) Then
                EngineObjectInternal.EngineObjectInstances = New HellionEngine.Support.DynamicArray(0)
            End If

            Me.Position = New HellionEngine.Support.Vector(0, 0)
            Me.Scale = New HellionEngine.Support.Vector(1, 1)
            Me.RotationCenter = New HellionEngine.Support.Vector(1, 1)
            Me.Rotation = New HellionEngine.Support.Rotation
            Me.Velocity = New HellionEngine.Support.Vector(0.0, 0.0)
            Me.Color = New HellionEngine.Support.Color(255, 255, 255, 255)
            Me.Visible = True

            EngineObjectInternal.EngineObjectInstances.Append(Me)
        End Sub

        Public ReadOnly Property Disposed() As Boolean Implements DisposableObject.Disposed
            Get
                Return Me.InternalDisposed
            End Get
        End Property

        Public Overridable Sub OnKeyPressed(Key As HellionEngine.Input.Key)

        End Sub

        Public Overridable Sub OnKeyReleased(Key As HellionEngine.Input.Key)

        End Sub

        Private Sub KeyPressedHandle(Key As HellionEngine.Input.Key)
            Me.OnKeyPressed(Key)
        End Sub

        Private Sub KeyReleasedHandle(Key As HellionEngine.Input.Key)
            Me.OnKeyReleased(Key)
        End Sub

        Public Overridable Sub OnCollision(Other As Physics.PhysicalObject) Implements Physics.PhysicalObject.OnCollision

        End Sub

        Public Overridable Sub Dispose() Implements HellionEngine.Core.DisposableObject.Dispose
            Me.InternalDisposed = True

            Me.UnbindKeyboardEvents()
        End Sub

        Public Overridable Sub OnDraw() Implements Rendering.RenderedObject.OnDraw

        End Sub

        REM Stuff
        Public Overridable Sub MouseAwayTick(MousePosition As Support.Vector) Implements InteractiveObject.MouseAwayTick

        End Sub

        Public Overridable Sub MouseOverTick(MousePosition As Support.Vector) Implements InteractiveObject.MouseOverTick

        End Sub

        Public Overridable Sub OnMouseButtonClicked(MouseButton As MouseButton, MousePosition As Support.Vector) Implements InteractiveObject.OnMouseButtonClicked

        End Sub

        Public Overridable Sub OnMouseButtonReleased(MouseButton As MouseButton, MousePosition As Support.Vector) Implements InteractiveObject.OnMouseButtonReleased

        End Sub

        Public Overridable Sub OnMousedAway(MousePosition As Support.Vector) Implements InteractiveObject.OnMousedAway

        End Sub

        Public Overridable Sub OnMousedOver(MousePosition As Support.Vector) Implements InteractiveObject.OnMousedOver
            RaiseEvent MousedOver(MousePosition)
        End Sub

        Public Sub BindKeyboardEvents()
            AddHandler HellionEngine.Input.Singleton.KeyPressed, AddressOf Me.KeyPressedHandle
            AddHandler HellionEngine.Input.Singleton.KeyReleased, AddressOf Me.KeyReleasedHandle
        End Sub

        Public Sub UnbindKeyboardEvents()
            RemoveHandler HellionEngine.Input.Singleton.KeyPressed, AddressOf Me.KeyPressedHandle
            RemoveHandler HellionEngine.Input.Singleton.KeyReleased, AddressOf Me.KeyReleasedHandle
        End Sub

        Public Overridable Property Dimensions() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(0, 0)
            End Get
            Set(value As HellionEngine.Support.Vector)

            End Set
        End Property

        Public Overridable ReadOnly Property Center() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Me.Position.X + ((Me.BoundingBox.Right - Me.BoundingBox.Left) / 2), Me.Position.Y + ((Me.BoundingBox.Bottom - Me.BoundingBox.Top) / 2))
            End Get
        End Property

        Public Overridable Property BoundingBox() As HellionEngine.Support.Rectangle
            Get
                Return New HellionEngine.Support.Rectangle(Me.Position.X, Me.Position.Y, Me.Dimensions.X, Me.Dimensions.Y)
            End Get
            Set(value As HellionEngine.Support.Rectangle)
                Me.Position = New HellionEngine.Support.Vector(value.Left, value.Right)
                Me.Dimensions = New HellionEngine.Support.Vector(value.Width, value.Height)
            End Set
        End Property

        REM Property that represents the radius of a circle that perfectly circumscribes this RenderedObject instance.
        Public Overridable Property BoundingCircle() As Single
            Get
                Return Math.Sqrt(Math.Pow(Me.BoundingBox.Left - Me.BoundingBox.Right, 2) + Math.Pow(Me.BoundingBox.Top - Me.BoundingBox.Bottom, 2)) / 2
            End Get
            Set(value As Single)

            End Set
        End Property

        Public Overridable Property Position() As HellionEngine.Support.Vector
            Get
                Return Me.InternalPosition
            End Get
            Set(value As HellionEngine.Support.Vector)
                Me.InternalPosition = value
            End Set
        End Property

        REM Gets the current forward vector of this drawn object.
        Public Overridable ReadOnly Property ForwardVector() As HellionEngine.Support.Vector
            Get
                Return New HellionEngine.Support.Vector(Math.Cos(Me.Rotation.Radians), Math.Sin(Me.Rotation.Radians))
            End Get
        End Property

        Public Overridable ReadOnly Property LeftVector() As HellionEngine.Support.Vector
            Get
                If (Me.Rotation.Radians >= 0 And Me.Rotation.Radians <= Math.PI) Then
                    Return New HellionEngine.Support.Vector(Math.Cos(Me.Rotation.Radians + (Math.PI / 2)), Math.Sin(Me.Rotation.Radians + (Math.PI / 2)))
                Else
                    Return New HellionEngine.Support.Vector(Math.Cos(Me.Rotation.Radians - (Math.PI / 2)), Math.Sin(Me.Rotation.Radians - (Math.PI / 2)))
                End If
            End Get
        End Property

        Public Overridable ReadOnly Property RightVector() As HellionEngine.Support.Vector
            Get
                If (Me.Rotation.Radians >= 0 And Me.Rotation.Radians <= Math.PI) Then
                    Return New HellionEngine.Support.Vector(Math.Cos(Me.Rotation.Radians - (Math.PI / 2)), Math.Sin(Me.Rotation.Radians - (Math.PI / 2)))
                Else
                    Return New HellionEngine.Support.Vector(Math.Cos(Me.Rotation.Radians + (Math.PI / 2)), Math.Sin(Me.Rotation.Radians + (Math.PI / 2)))
                End If
            End Get
        End Property

        Public Overridable Sub OnUpdate(DeltaTimeSeconds As Single) Implements InteractiveObject.OnUpdate
            Dim CurrentMousePosition As HellionEngine.Support.Vector = HellionEngine.Input.Singleton.GetMousePosition()

            REM Handle the OnMouseOver/OnMouseAway subroutines.
            If (Me.BoundingBox.Contains(CurrentMousePosition.X, CurrentMousePosition.Y)) Then
                Me.OnMousedOver(CurrentMousePosition)

                If (HellionEngine.Input.Singleton.MouseButtonPressed(HellionEngine.Input.MouseButton.Left)) Then
                    Me.OnMouseButtonClicked(HellionEngine.Input.MouseButton.Left, CurrentMousePosition)
                Else
                    Me.OnMouseButtonReleased(HellionEngine.Input.MouseButton.Left, CurrentMousePosition)
                End If

                ' If (BattleZone.InputManager.MouseButtonPressed(BattleZone.InputManager.MouseButton.Right)) Then
                ''  Me.OnMouseClicked(CurrentMousePosition, BattleZone.InputManager.MouseButton.Right)
                ' Else
                '    Me.OnMouseReleased(CurrentMousePosition, BattleZone.InputManager.MouseButton.Right)
                'End If

                '   If (BattleZone.InputManager.MouseButtonPressed(BattleZone.InputManager.MouseButton.Middle)) Then
                'Me.OnMouseClicked(CurrentMousePosition, BattleZone.InputManager.MouseButton.Middle)
                '   Else
                '    Me.OnMouseReleased(CurrentMousePosition, BattleZone.InputManager.MouseButton.Middle)
                ' End If
            Else
                Me.OnMousedAway(CurrentMousePosition)
            End If
        End Sub

        Public Overridable Sub OnPhysicsUpdate(DeltaTimeSeconds As Single) Implements Physics.PhysicalObject.OnUpdate

        End Sub

        Public Overridable Function TestCollision(Other As EngineObject) As Boolean Implements PhysicalObject.TestCollision
            Return False
        End Function

        REM Raisable Events
        Public Event MousedOver(MousePosition As HellionEngine.Support.Vector)

        REM Unused at this level
        Public Property RenderSystem As RenderLayerSystem Implements RenderedObject.RenderSystem
        Public Property Color As Support.Color Implements RenderedObject.Color
        Public Property InternalPosition As Support.Vector Implements RenderedObject.Position
        Public Property Rotation As Support.Rotation Implements RenderedObject.Rotation
        Public Property Scale As Support.Vector Implements RenderedObject.Scale
        Public Property RotationCenter As Support.Vector Implements RenderedObject.RotationCenter
        Public Property Visible As Boolean Implements RenderedObject.Visible

        Public Property Velocity As Support.Vector Implements PhysicalObject.Velocity

        Public Property Mass As Single Implements PhysicalObject.Mass

        Public Property RotationVelocity As Single Implements PhysicalObject.RotationVelocity
    End Class
End Namespace
