Public Module Interaction
    Private SingletonInstance As ClassInstance

    Public Function Instantiate() As ClassInstance
        If (SingletonInstance Is Nothing) Then
            SingletonInstance = New ClassInstance
        End If

        Return SingletonInstance
    End Function

    Public ReadOnly Property Singleton() As ClassInstance
        Get
            Return SingletonInstance
        End Get
    End Property

    Public Class ClassInstance
        Inherits HellionEngine.Core.EngineObject

        Public InteractiveObjects As HellionEngine.Support.DynamicArray

        Public Sub New()
            Me.InteractiveObjects = New HellionEngine.Support.DynamicArray(0)
            HellionEngine.Logging.Write("Interaction.vb [ClassInstance+New]: Initialization successful.")
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.InteractiveObjects.Dispose()
            HellionEngine.Logging.Write("Interaction.vb [ClassInstance+Dispose]: Successfully deinitialized.")
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            Me.InteractiveObjects.IterationBegin(False)
            While (Not Me.InteractiveObjects.IterationEnd())
                Dim InteractiveRef As InteractiveObject = Me.InteractiveObjects.IterationNext()
                InteractiveRef.OnUpdate(DeltaTimeSeconds)
            End While
        End Sub
    End Class

    Public Interface InteractiveObject
        Inherits HellionEngine.Core.DisposableObject

        Sub OnUpdate(DeltaTimeSeconds As Single)

        Sub OnMouseButtonClicked(MouseButton As HellionEngine.Input.MouseButton, MousePosition As HellionEngine.Support.Vector)
        Sub OnMouseButtonReleased(MouseButton As HellionEngine.Input.MouseButton, MousePosition As HellionEngine.Support.Vector)
        Sub OnMousedOver(MousePosition As HellionEngine.Support.Vector)
        Sub OnMousedAway(MousePosition As HellionEngine.Support.Vector)
        Sub MouseOverTick(MousePosition As HellionEngine.Support.Vector)
        Sub MouseAwayTick(MousePosition As HellionEngine.Support.Vector)
    End Interface
End Module