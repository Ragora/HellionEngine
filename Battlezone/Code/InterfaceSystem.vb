Public Module InterfaceSystem
    Private ActiveInterface As InterfaceBase
    Private RegisteredInterfaces As New HellionEngine.Support.DynamicArray(0)

    Public Property ActiveInterfaceName() As String
        Get
            Return ActiveInterface.Name
        End Get
        Set(value As String)
            Dim RequestedInterface = GetInterface(value)

            If (RequestedInterface Is Nothing) Then
                HellionEngine.Logging.Write("Battlezone Interfaces.vb [ActiveInterfaceName]: No such Interface: '" + value + "'")
                Return
            End If

            REM Handle the old interface callbacks
            Dim OldInterfaceName As String = "<NONE>"
            If (Not (ActiveInterface Is Nothing)) Then
                ActiveInterface.OnSleep(value)
                OldInterfaceName = ActiveInterface.Name
            End If

            ActiveInterface = RequestedInterface
            ActiveInterface.OnWake(OldInterfaceName)

            HellionEngine.Rendering.Singleton.LayeringSystem = ActiveInterface.RenderLayerSystem
            HellionEngine.Interaction.Singleton.InteractiveObjects = ActiveInterface.UpdateObjectList
        End Set
    End Property

    Public Function GetInterface(Name As String)
        RegisteredInterfaces.IterationBegin(False)
        While (Not RegisteredInterfaces.IterationEnd())
            Dim CurrentInterface As InterfaceBase = RegisteredInterfaces.IterationNext()

            If (CurrentInterface.Name = Name) Then
                Return CurrentInterface
            End If
        End While

        Return Nothing
    End Function

    Public MustInherit Class InterfaceBase
        Inherits HellionEngine.Core.EngineObject

        Private InternalName As String
        Public RenderLayerSystem As HellionEngine.Rendering.RenderLayerSystem
        Public UpdateObjectList As HellionEngine.Support.DynamicArray

        Public Sub New(Name As String, RenderLayerCount As Integer)
            MyBase.New()

            Me.InternalName = Name

            Me.UpdateObjectList = New HellionEngine.Support.DynamicArray(0)
            Me.UpdateObjectList.Append(Me)
            Me.RenderLayerSystem = New HellionEngine.Rendering.RenderLayerSystem(RenderLayerCount)

            If (GetInterface(Name) Is Nothing) Then
                RegisteredInterfaces.Append(Me)
            Else
                HellionEngine.Logging.Write("Battlezone InterfaceSystem.vb [InterfaceBase+New]: Attempted to recreate Interface '" + Name + "'!")
            End If
        End Sub

        Public Overridable Sub OnWake(OldInterfaceName As String)
            Me.BindKeyboardEvents()
        End Sub

        Public Overridable Sub OnSleep(NewInterfaceName As String)
            Me.UnbindKeyboardEvents()
        End Sub

        Public Overrides Sub Dispose()
            MyBase.Dispose()

            Me.RenderLayerSystem.Dispose()
            REM TODO: Fix Stack Overflow Exception
            REM Me.UpdateObjectList.Dispose()
        End Sub

        Public ReadOnly Property Name() As String
            Get
                Return New String(Me.InternalName)
            End Get
        End Property
    End Class
End Module
