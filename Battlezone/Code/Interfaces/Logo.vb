Namespace Interfaces
    Public Class Logo
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Private Logo As HellionEngine.Game.Sprite

        Public Sub New()
            MyBase.New("Logo", 1)

            Me.Logo = New HellionEngine.Game.Sprite("Textures/Interface/Draconic.png")
            Me.Logo.Position = New HellionEngine.Support.Vector(270, 270)
            Me.Logo.Scale = New HellionEngine.Support.Vector(2, 2)
            Me.Logo.Color.Alpha = 0
            Me.Logo.Visible = True

            Me.RenderLayerSystem.Append(0, Logo)
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            If (Me.Logo.Color.Alpha >= 250) Then
                Me.Logo.Color.Alpha = 255
                Return
            Else
                Me.Logo.Color.Alpha += 60 * DeltaTimeSeconds
            End If
        End Sub

        Public Overrides Sub OnWake(OldInterfaceName As String)
            MyBase.OnWake(OldInterfaceName)

            Me.Logo.Color.Alpha = 0

#If DEBUG Then
            Me.SwitchTimerFire_Handle()
#Else
            ' Dim VoiceFire As New HellionEngine.Support.Timing.TimedEvent(4000, 0)
            ' AddHandler VoiceFire.OnFire, AddressOf Me.VoiceTimer_Fire
            Dim SwitchFire As New HellionEngine.Support.Timing.TimedEvent(7000, 0)
            AddHandler SwitchFire.OnFire, AddressOf Me.SwitchTimerFire_Handle
#End If
        End Sub

        Public Sub SwitchTimerFire_Handle()
            Battlezone.ActiveInterfaceName = "Start"
        End Sub
    End Class
End Namespace