Namespace Interfaces
    Public Class Play
        Inherits Battlezone.InterfaceSystem.InterfaceBase

        Public Game As Battlezone.GameModes.BZGame

        Private Logo As HellionEngine.Game.Sprite

        Public Sub New()
            MyBase.New("Play", 6)

            Me.RenderLayerSystem.HasWorldRender = True
        End Sub

        Public Overrides Sub OnWake(OldInterfaceName As String)
            MyBase.OnWake(OldInterfaceName)

            Me.BindKeyboardEvents()
            Me.Game.BindKeyboardEvents()
        End Sub

        Public Overrides Sub OnSleep(NewInterfaceName As String)
            MyBase.OnSleep(NewInterfaceName)

            Me.UnbindKeyboardEvents()
            Me.Game.UnbindKeyboardEvents()
        End Sub

        Public Overrides Sub OnDraw()
            MyBase.OnDraw()

            System.Console.WriteLine("DRAW PLAYUI")
        End Sub

        Public Overrides Sub OnUpdate(DeltaTimeSeconds As Single)
            MyBase.OnUpdate(DeltaTimeSeconds)

            REM Physics and AI shouldn't be updated unless our PlayGUI is active (passive pausing, pretty much)
            HellionEngine.Physics.Singleton.OnUpdate(DeltaTimeSeconds)

            Me.Game.OnUpdate(DeltaTimeSeconds)
        End Sub

        Public Overrides Sub OnKeyPressed(Key As HellionEngine.Key)
            MyBase.OnKeyPressed(Key)

            If (Key = HellionEngine.Input.Key.Escape) Then
                Battlezone.InterfaceSystem.ActiveInterfaceName = "Escape"
            End If
        End Sub
    End Class
End Namespace